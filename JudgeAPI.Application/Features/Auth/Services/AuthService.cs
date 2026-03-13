namespace JudgeAPI.Application.Features;
using JudgeAPI.Domain;
using JudgeAPI.Application.Common;

using AutoMapper;

public class AuthService : IAuthService
{
    private readonly IMapper _mapper;
    private readonly ICurrentUserService _currentUserService;
    private readonly ITokenService _tokenService;
    private readonly IIdentityService _identityService;
    private readonly ISubmissionRepository _submissionRepository;

    public AuthService(
            IMapper mapper,
            ICurrentUserService currentUserService,
            ITokenService tokenService,
            IIdentityService identityService,
            ISubmissionRepository submissionRepository
            )
    {
        _mapper = mapper;
        _currentUserService = currentUserService;
        _tokenService = tokenService;
        _identityService = identityService;
        _submissionRepository = submissionRepository;
    }

    /*
       POST /logout
       POST /refresh-token (si usás JWT con refresh tokens)
       POST /confirm-email
       POST /forgot-password
       POST /reset-password
*/

    // ---- REGISTER ---- //
    public async Task<TokenResponseDTO> RegisterAsync(UserCreateDTO dto)
    {
        var userExist = await _identityService.FindByNameAsync(dto.Username);

        if (userExist is not null)
            throw new ConflictException("El nombre del usuario ya está en uso.");
        
        var newUser = new UserDTO {
            UserName = dto.Username,
            Email = dto.Username,
           FirstName = dto.FirstName,
           LastName = dto.LastName,
           University = dto.Universidad,
           IsActive = true
        };

        var result = await _identityService.CreateUserAsync(newUser, dto.Password);

        if (!result.Succeeded){
            var errors = string.Join(" | ", result.Errors!);
            throw new Exception($"Error al crear el usuario: {errors}");
        }

        // Roles. Si no existe lo creamos la primera vez.
        var roleExists = await _identityService.RoleExistsAsync(Roles.Student);

        if (!roleExists)
            await _identityService.CreateRoleAsync(Roles.Student);

        await _identityService.AddRoleAsync(newUser, Roles.Student);

        // Obtenemos Roles y token para colocar en la respuesta
        var roles = await _identityService.GetRoleAsync(newUser);
        var token = _tokenService.GenerateToken(newUser.Id, newUser.Email, roles!);

        return new TokenResponseDTO
        {
            Token = token,
            UserId = newUser.Id!,
            UserName = newUser.UserName ?? "",
            FirstName = newUser.FirstName,
            LastName = newUser.LastName,
            Email = newUser.Email,
            Roles = roles!.ToList()
        };
    }

    // ---- LOGIN ---- //
    public async Task<TokenResponseDTO> LoginAsync(LoginRequestDTO request)
    {
        var user = await _identityService.FindByNameAsync(request.UserName);

        if (user is null)
            throw new ConflictException("Usuario o contraseña incorrectos");

        var passwordValid = await _identityService.CheckPasswordAsync(user.Email, request.Password);

        if (!passwordValid)
            throw new ConflictException("Usuario o contraseña incorrectos");

        // Roles y token para armar la  respuestas
        var roles = await _identityService.GetRoleAsync(user);
        var token = _tokenService.GenerateToken(user.Id, user.Email, roles!);

        var submissionList = await _submissionRepository.GetAllByUserIdAsync(user.Id);

        var tokenResponse = _mapper.Map<TokenResponseDTO>(user);
        tokenResponse.Submissions = _mapper.Map<List<SubmissionResponseDTO>>(submissionList);
        tokenResponse.Token = token;
        tokenResponse.UserId = user.Id!;
        tokenResponse.Roles = roles!.ToList();

        return tokenResponse;
    }
}

