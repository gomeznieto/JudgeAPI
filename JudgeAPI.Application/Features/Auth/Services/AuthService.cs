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

namespace JudgeAPI.Application.Features.Auth.Services
{
    public class AuthService(
            IMapper mapper,
            ITokenService tokenService,
            IIdentityService identityService,
            ISubmissionRepository submissionRepository
                ) : IAuthService
    {
        private readonly IMapper _mapper = mapper;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IIdentityService _identityService = identityService;
        private readonly ISubmissionRepository _submissionRepository = submissionRepository;

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

            string? email = newUser?.Email;
            string token = _tokenService.GenerateToken(newUser.Id, email, roles!);

            return new TokenResponseDTO
            {
                Token = token,
                UserId = newUser.Id!,
                UserName = newUser.UserName ?? "",
                FirstName = newUser.FirstName,
                LastName = newUser.LastName,
                Email = newUser.Email,
                Roles = [.. roles]
            };
        }

        // ---- LOGIN ---- //
        public async Task<TokenResponseDTO> LoginAsync(LoginRequestDTO request)
        {
            UserDTO? user = await _identityService.FindByNameAsync(request.UserName) ?? throw new ConflictException("Usuario o contraseña incorrectos");
            Console.WriteLine($"{request.UserName} {request.Password}");
            bool passwordValid = await _identityService.CheckPasswordAsync(user.UserName, request.Password);

            if (!passwordValid)
            {
                throw new ConflictException("Usuario o contraseña incorrectos");
            }

            // Roles y token para armar la  respuestas
            IList<string> roles = await _identityService.GetRoleAsync(user) ?? [];
            string token = _tokenService.GenerateToken(user.Id, user.Email, roles);

            List<Submission> submissionList = await _submissionRepository.GetAllByUserIdAsync(user.Id);

            TokenResponseDTO tokenResponse = _mapper.Map<TokenResponseDTO>(user);
            tokenResponse.Submissions = _mapper.Map<List<SubmissionResponseDTO>>(submissionList);
            tokenResponse.Token = token;
            tokenResponse.UserId = user.Id;
            tokenResponse.Roles = [.. roles];

            return tokenResponse;
        }
    }

}
