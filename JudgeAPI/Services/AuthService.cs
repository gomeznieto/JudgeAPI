using AutoMapper;
using JudgeAPI.Data;
using JudgeAPI.Entities;
using JudgeAPI.Excerptions;
using JudgeAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JudgeAPI.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _appDbContext;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        public AuthService(
            AppDbContext appDbContext,
            IMapper mapper,
            UserManager<ApplicationUser> userManager
        )
        {
            _appDbContext = appDbContext;
            _mapper = mapper;
            _userManager = userManager;
        }

        /*
            POST /login

            POST /logout

            POST /refresh-token (si usás JWT con refresh tokens)

            POST /confirm-email

            POST /forgot-password

            POST /reset-password
         
         */

        // REGISTER
        public async Task<UserResponseDTO> RegisterAsync(UserCreateDTO dto)
        {
            var userExist = await _userManager.FindByNameAsync(dto.Username);

            if (userExist is not null)
                throw new ConflictException("El nombre del usuario ya está en uso.");

            var emailExist = await _userManager.FindByEmailAsync(dto.Email!);

            if (emailExist is not null)
                throw new ConflictException("El email ya está en uso");

            var user = _mapper.Map<ApplicationUser>(dto);

            var result = await _userManager.CreateAsync(user, dto.Password);

            if(!result.Succeeded)
            {
                var errors = string.Join(" | ", result.Errors.Select(e => e.Description));
                throw new Exception($"Error al crear el usuario: {errors}");
            }

            return _mapper.Map<UserResponseDTO>(user);
        }
    }
}
