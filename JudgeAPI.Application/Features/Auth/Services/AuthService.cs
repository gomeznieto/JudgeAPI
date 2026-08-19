using JudgeAPI.Application.Features.Submissions.Interfaces;

using AutoMapper;
using JudgeAPI.Application.Features.Submissions.Dtos;
using JudgeAPI.Application.Features.Auth.Dtos;
using JudgeAPI.Application.Features.Auth.Iterfaces;
using JudgeAPI.Application.Common.Interfaces;
using JudgeAPI.Domain.Entities;
using JudgeAPI.Domain.Constants;
using JudgeAPI.Application.Common.Exceptions;
using JudgeAPI.Application.Common.Dtos;
using JudgeAPI.Application.Features.Users.Dtos;
using System.Security.Claims;

namespace JudgeAPI.Application.Features.Auth.Services
{
  public class AuthService(
      IMapper mapper,
      ITokenService tokenService,
      IIdentityService identityService,
      ISubmissionRepository submissionRepository,
      IRefreshTokenRepository refreshTokenRepository,
      IUnitOfWork unitOfWork
      ) : IAuthService
  {
    private readonly IMapper _mapper = mapper;
    private readonly ITokenService _tokenService = tokenService;
    private readonly IIdentityService _identityService = identityService;
    private readonly ISubmissionRepository _submissionRepository = submissionRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    // ---- REGISTER ---- //
    public async Task<TokenResponseDTO> RegisterAsync(UserCreateDTO dto)
    {
      UserDTO? userAlreadyExist = await _identityService.FindByNameAsync(dto.Username);

      if (userAlreadyExist is not null)
      {
        throw new ConflictException("El nombre del usuario ya está en uso.");
      }

      // Creamos al usuario con los datos del DTO
      UserDTO newUser = new()
      {
        UserName = dto.Username,
        Email = dto.Email,
        FirstName = dto.FirstName,
        LastName = dto.LastName,
        University = dto.Universidad,
        IsActive = true
      };

      IdentityResultDTO result = await _identityService.CreateUserAsync(newUser, dto.Password);

      if (!result.Succeeded)
      {
        string errors = string.Join(" | ", result.Errors!);
        throw new Exception($"Error al crear el usuario: {errors}");
      }

      // Roles. Si no existe lo creamos la primera vez.
      bool roleExists = await _identityService.RoleExistsAsync(Roles.Student);

      if (!roleExists)
      {
        _ = await _identityService.CreateRoleAsync(Roles.Student);
      }

      await _identityService.AddRoleAsync(newUser, Roles.Student);

      // Obtenemos Roles y token para colocar en la respuesta
      IList<string> roles = await _identityService.GetRoleAsync(newUser) ?? [];

      string token = _tokenService.GenerateToken(newUser.Id, newUser.UserName, roles!);
      string refreshToken = _tokenService.GenerateRefreshToken();
      
      Console.ForegroundColor = ConsoleColor.Green;
      Console.WriteLine($"[AuthService] Token generado para el usuario {newUser.Id}: {token
}");
      UserRefreshToken userRefreshToken = new()
      {
        UserId = Guid.Parse(newUser.Id),
        TokenHash = _tokenService.GetHashToken(refreshToken),
        ExpiresAt = DateTime.UtcNow.AddDays(7),
        IsRevoked = false
      };

      _refreshTokenRepository.Add(userRefreshToken);
      await _unitOfWork.SaveChangesAsync();

      return new TokenResponseDTO
      {
        Token = token,
        RefreshToken = refreshToken,
        UserId = newUser.Id!,
        UserName = newUser.UserName!,
        FirstName = newUser.FirstName,
        LastName = newUser.LastName,
        Email = newUser.Email,
        University = newUser.University,
        Roles = [.. roles] 
      };
    }

    // ---- LOGIN ---- //
    public async Task<TokenResponseDTO> LoginAsync(LoginRequestDTO request)
    {
      UserDTO? user = await _identityService.FindByNameAsync(request.UserName) ?? throw new ConflictException("Usuario o contraseña incorrectos");
      bool passwordValid = await _identityService.CheckPasswordAsync(user.UserName, request.Password);

      if (!passwordValid)
      {
        throw new ConflictException("Usuario o contraseña incorrectos");
      }

      // Datos para generar el token y el refresh token
      IList<string> roles = await _identityService.GetRoleAsync(user) ?? [];
      string token = _tokenService.GenerateToken(user.Id, user.Email, roles);
      string refreshToken = _tokenService.GenerateRefreshToken();
      List<Submission> submissionList = await _submissionRepository.GetAllByUserIdAsync(user.Id);

            // Guardamos el token de actualización en la base de datos
      UserRefreshToken userRefreshToken = new()
      {
        UserId = Guid.Parse(user.Id),
        TokenHash = _tokenService.GetHashToken(refreshToken),
        ExpiresAt = DateTime.UtcNow.AddDays(7),
        IsRevoked = false
      };

      _refreshTokenRepository.Add(userRefreshToken);

      await _unitOfWork.SaveChangesAsync();

      // Mapeamos el usuario y las submissions a DTOs
      TokenResponseDTO tokenResponse = _mapper.Map<TokenResponseDTO>(user);
      tokenResponse.Submissions = _mapper.Map<List<SubmissionResponseDTO>>(submissionList);
      tokenResponse.Token = token;
      tokenResponse.RefreshToken = refreshToken;
      tokenResponse.UserId = user.Id;
      tokenResponse.Roles = [.. roles];

      return tokenResponse;
    }

    // ---- REFRESH TOKEN ---- //
    // Implementa Refresh Token Rotation:
    // 1. Valida y revoca el token actual (un solo uso).
    // 2. Emite y persiste un nuevo par (Access + Refresh Token) para mitigar robo o reutilización.
    public async Task<TokenResponseDTO> RefreshTokenAsync(TokenRequestDTO dto)
    {
      // Obtenemos los Claims
      IEnumerable<Claim> userClaims = _tokenService.GetPrincipalFromExpiredToken(dto.Token).Claims;
      string? userId = userClaims.FirstOrDefault(static c => c.Type == ClaimTypes.NameIdentifier)?.Value;

      if (string.IsNullOrEmpty(userId))
      {
        throw new Exception("No se pudo obtener la información del usuario.");
      }

      // Obtenemos el usuario de la base de datos
      UserDTO user = await _identityService.FindByIdAsync(userId) ?? throw new Exception("Usuario no encontrado");

      // Parseamos el token de actualización para obtener el hash
      string parseToken = _tokenService.GetHashToken(dto.RefreshToken);

      // Validamos que la expiración dele RefreshToken no esté vencida
      UserRefreshToken? userRefreshToken = await _refreshTokenRepository.GetByUserIdAndHashAsync(userId, parseToken) ?? throw new UnauthorizedAccessException("Error: ID de usuario o token de actualización no válidos.");

      if (userRefreshToken.ExpiresAt < DateTime.UtcNow || userRefreshToken.IsRevoked)
      {
        throw new UnauthorizedAccessException("Error: Token de actualización expirado o revocado.");
      }

      // Marcamos el token de actualización como revocadoj
      userRefreshToken.ExpiresAt = DateTime.UtcNow;
      userRefreshToken.IsRevoked = true;

      _refreshTokenRepository.Update(userRefreshToken);

      // Generamos un nuevo token y refresh token
      IList<string> roles = await _identityService.GetRoleAsync(user) ?? [];
      string refreshToken = _tokenService.GenerateRefreshToken();
      string newToken = _tokenService.GenerateToken(user.Id, user.UserName, roles);

      // Actualizamos el token de actualización en la base de datos
      userRefreshToken.TokenHash = _tokenService.GetHashToken(refreshToken);
      userRefreshToken.ExpiresAt = DateTime.UtcNow.AddDays(7);
      _refreshTokenRepository.Add(userRefreshToken);

      await _unitOfWork.SaveChangesAsync();

      return new TokenResponseDTO
      {
        Token = newToken,
        RefreshToken = refreshToken,
        UserId = user.Id,
        UserName = user.UserName,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        University = user.University,
        Roles = [.. roles]
      };

    }

  }

}
