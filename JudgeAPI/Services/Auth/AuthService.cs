using AutoMapper;
using JudgeAPI.Data;
using JudgeAPI.Entities;
using JudgeAPI.Excerptions;
using JudgeAPI.Models.Auth;
using JudgeAPI.Models.User;
using JudgeAPI.Services.Token;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JudgeAPI.Services.Ath
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ITokenService _tokenService
            ;

        public AuthService(
            AppDbContext appDbContext,
            IMapper mapper,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ITokenService tokenService
        )
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _userManager = userManager;
            _roleManager = roleManager;
            _tokenService = tokenService;
        }

        /*
            POST /logout

            POST /refresh-token (si usás JWT con refresh tokens)

            POST /confirm-email

            POST /forgot-password

            POST /reset-password
         
         */

        // REGISTER
        public async Task<TokenResponse> RegisterAsync(UserCreateDTO dto)
        {
            var userExist = await _userManager.FindByNameAsync(dto.Username);

            if (userExist is not null)
                throw new ConflictException("El nombre del usuario ya está en uso.");

            var user = _mapper.Map<ApplicationUser>(dto);

            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new Exception($"Error al crear el usuario: {errors}");
            }

            // Roles. Si no existe lo creamos la primera vez.
            var roleExists = await _roleManager.RoleExistsAsync("User");
            if (!roleExists)
                await _roleManager.CreateAsync(new IdentityRole("User"));

            await _userManager.AddToRoleAsync(user, "User");

            // Obtenemos Roles y token para colocar en la respuesta
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            return new TokenResponse
            {
                Token = token,
                UserId = user.Id!,
                UserName = user.UserName ?? "",
                Roles = roles.ToList()
            };
        }

        // LOGIN
        public async Task<TokenResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user is null)
                throw new ConflictException("Usuario o contraseña incorrectos");

            var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!passwordValid)
                throw new ConflictException("Usuario o contraseña incorrectos");

            // Roles y token para armar la  respuestas
            var roles = await _userManager.GetRolesAsync(user);
            var token = _tokenService.GenerateToken(user, roles);

            return new TokenResponse
            {
                Token = token,
                UserId = user.Id!,
                UserName = user.UserName ?? "",
                Roles = roles.ToList()
            };
        }
    }
}
