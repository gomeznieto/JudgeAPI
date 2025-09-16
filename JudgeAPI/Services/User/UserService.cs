using AutoMapper;
using JudgeAPI.Constants;
using JudgeAPI.Data;
using JudgeAPI.Entities;
using JudgeAPI.Excerptions;
using JudgeAPI.Models.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace JudgeAPI.Services.User
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly AppDbContext _dbContext;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public UserService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext dbContext,
            ICurrentUserService currentUserService,
            IMapper mapper
            )
        {
            _userManager = userManager;
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        // GET BY ID
        public async Task<UserResponseDTO> GetUserByIdAsync(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user is null) throw new NotFoundException($"El usuario no encontrado");

            var roles = await _userManager.GetRolesAsync(user);

            var currentUserId = _currentUserService.GetCurrentUserId();


            var currentUserRoles = _currentUserService.GetCurrentUserRole();


            if (roles.Contains(Roles.Admin) && !currentUserRoles.Contains(Roles.Admin))

            if (user.Id == currentUserId)
            {
                return _mapper.Map<UserPrivateDTO>(user);
            }

            return _mapper.Map<UserPublicDTO>(user);
        }
    }
}
