using JudgeAPI.Application.Common.Dtos;
using JudgeAPI.Application.Common.Interfaces;
using JudgeAPI.Application.Features;
using JudgeAPI.Application.Features.Users.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace JudgeAPI.Infrastructure.Identity
{
    public sealed class IdentityService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager
            )
        : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly RoleManager<IdentityRole> _roleManager = roleManager;

        // USER METHODS
        public async Task<bool> CheckPasswordAsync(string email, string password)
        {
            ApplicationUser? user = await _userManager.FindByEmailAsync(email);
            return user is not null && await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IdentityResultDTO> CreateUserAsync(UserDTO user, string password)
        {
            ApplicationUser applicationUser = new()
            {
                UserName = user.UserName,
                Email = user.UserName,
                FirstName = user.FirstName ?? null,
                LastName = user.LastName ?? null,
                University = user.University ?? null
            };

            IdentityResult result = await _userManager.CreateAsync(applicationUser, password);

            return new IdentityResultDTO
            {
                Succeeded = result.Succeeded,
                Errors = result.Errors.Select(static e => e.Description),
            };
        }

        public async Task<UserDTO?> FindByNameAsync(string username)
        {
            ApplicationUser? applicationUser = await _userManager.FindByNameAsync(username);

            return applicationUser is not null ? new UserDTO
            {
                Id = applicationUser.Id,
                UserName = applicationUser.UserName!,
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName,
                University = applicationUser.University
            } : null;
        }

        public async Task<IdentityResultDTO> ChangePasswordAsync(string id, ChangePasswordDTO dto)
        {
            ApplicationUser? user = await _userManager.FindByIdAsync(id);
            IdentityResult result = await _userManager.ChangePasswordAsync(user!, dto.OldPassword, dto.NewPassword);

            return new IdentityResultDTO
            {
                Succeeded = result.Succeeded,
                Errors = result.Errors.Select(static e => e.Description),
            };
        }

        // ROLE
        public async Task AddRoleAsync(UserDTO user, string role)
        {
            ApplicationUser applicationUser = new()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.UserName,
                FirstName = user.FirstName ?? null,
                LastName = user.LastName ?? null,
                University = user.University ?? null
            };

            _ = await _userManager.AddToRoleAsync(applicationUser, role);
        }

        public async Task<IdentityResultDTO> CreateRoleAsync(string role)
        {
            IdentityResult result = await _roleManager.CreateAsync(new IdentityRole()
            { Name = role });

            return new IdentityResultDTO
            {
                Succeeded = result.Succeeded,
                Errors = result.Errors.Select(static e => e.Description),
            };
        }

        public async Task<IList<string>?> GetRoleAsync(UserDTO user)
        {
            ApplicationUser applicationUser = new()
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.UserName,
                FirstName = user.FirstName ?? null,
                LastName = user.LastName ?? null,
                University = user.University ?? null
            };

            return await _userManager.GetRolesAsync(applicationUser);
        }

        public async Task<bool> RoleExistsAsync(string role)
        {
            return await _roleManager.RoleExistsAsync(role);
        }

        public async Task<UserDTO?> FindByIdAsync(string id)
        {
            ApplicationUser? applicationUser = await _userManager.FindByIdAsync(id);

            return applicationUser is not null ? new UserDTO
            {
                Id = applicationUser.Id,
                UserName = applicationUser.UserName!,
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName,
                University = applicationUser.University
            } : null;
        }

        public async Task<IdentityResultDTO> UpdateUserAsync(UserUpdateDTO user, string id)
        {
            ApplicationUser applicationUser = new()
            {
                Id = id,
                FirstName = user.FirstName ?? null,
                LastName = user.LastName ?? null,
                University = user.University ?? null
            };

            IdentityResult result = await _userManager.UpdateAsync(applicationUser);

            return new IdentityResultDTO
            {
                Succeeded = result.Succeeded,
                Errors = result.Errors.Select(static e => e.Description),
            };
        }

        public async Task<List<string?>> GetAllRolesAsync()
        {
            return await _roleManager.Roles.Select(static r => r.Name).ToListAsync() ?? [];
        }

        public async Task<bool> IsInRolAsync(UserDTO user, string role)
        {
            ApplicationUser? applicationUser = await _userManager.FindByIdAsync(user.Id);
            return applicationUser is not null && await _userManager.IsInRoleAsync(applicationUser, role);
        }

        public async Task<IdentityResultDTO> RemoveFromRoleAsync(UserDTO user, string role)
        {

            ApplicationUser? applicationUser = await _userManager.FindByIdAsync(user.Id);
            IdentityResult result = await _userManager.RemoveFromRoleAsync(applicationUser!, role);

            return new IdentityResultDTO
            {
                Succeeded = result.Succeeded,
                Errors = result.Errors.Select(static e => e.Description),
            };
        }
    }
}
