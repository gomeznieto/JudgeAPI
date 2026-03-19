using JudgeAPI.Application.Common;
using JudgeAPI.Application.Common.Interfaces;
using JudgeAPI.Application.Features;
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
            var user = await _userManager.FindByEmailAsync(email);
            if(user is null) return false;

            return await _userManager.CheckPasswordAsync(user, password);
        }

        public async Task<IdentityResultDTO> CreateUserAsync(Application.Features.UserDTO user, string password)
        {
            var applicationUser = new ApplicationUser {
                UserName = user.UserName,
                Email = user.UserName,
                FirstName = user.FirstName ?? null,
                LastName = user.LastName ?? null,
                University = user.University ?? null
            };

            var result =  await _userManager.CreateAsync(applicationUser, password);

            return new IdentityResultDTO{
                Succeeded = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description),
            };
        }

        public async Task<Application.Features.UserDTO?> FindByNameAsync(string username)
        {
            var applicationUser = await _userManager.FindByNameAsync(username);
            if(applicationUser is null) return null;

            return new UserDTO{
                Id = applicationUser.Id,
                UserName = applicationUser.UserName!,
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName,
                University = applicationUser.University
            };
        }

        public async Task<IdentityResultDTO>ChangePasswordAsync(string id, ChangePasswordDTO dto){
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.ChangePasswordAsync(user!, dto.OldPassword, dto.NewPassword);
            return new IdentityResultDTO{
                Succeeded = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description),
            };
        }


        // ROLE
        public async Task AddRoleAsync(Application.Features.UserDTO user, string role)
        {
            var applicationUser = new ApplicationUser {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.UserName,
                FirstName = user.FirstName ?? null,
                LastName = user.LastName ?? null,
                University = user.University ?? null
            };

            await _userManager.AddToRoleAsync(applicationUser, role); }

        public async Task<IdentityResultDTO> CreateRoleAsync(string role)
        {
            var result = await _roleManager.CreateAsync(new IdentityRole(){Name = role});

            return new IdentityResultDTO{
                Succeeded = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description),
            };

        }

        public async Task<IList<string>?> GetRoleAsync(Application.Features.UserDTO user)
        {
            var applicationUser = new ApplicationUser {
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
            var applicationUser = await _userManager.FindByIdAsync(id);
            if(applicationUser is null) return null; 

            return new UserDTO{
                Id = applicationUser.Id,
                UserName = applicationUser.UserName!,
                FirstName = applicationUser.FirstName,
                LastName = applicationUser.LastName,
                University = applicationUser.University
            };
        }

        public async Task<IdentityResultDTO> UpdateUserAsync(UserUpdateDTO user, string id){
            var applicationUser = new ApplicationUser {
                Id = id,
                FirstName = user.FirstName ?? null,
                LastName = user.LastName ?? null,
                University = user.University ?? null
            };

            var result = await _userManager.UpdateAsync(applicationUser);

            return new IdentityResultDTO{
                Succeeded = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description),
            };
        }

        public async Task <IList<string>> GetAllRolesAsync(){
            return await _roleManager.Roles.Select( r => r.Name).ToListAsync() ?? [];
        }

        public async Task<bool> IsInRolAsync(UserDTO user, string role){
            var applicationUser = await _userManager.FindByIdAsync(user.Id);
            if (applicationUser is null) return false;
            return await _userManager.IsInRoleAsync(applicationUser, role);
        }

        public async Task<IdentityResultDTO> RemoveFromRoleAsync(UserDTO user, string role){

            var applicationUser = await _userManager.FindByIdAsync(user.Id);
            var result = await _userManager.RemoveFromRoleAsync(applicationUser!, role);
            return new IdentityResultDTO{
                Succeeded = result.Succeeded,
                Errors = result.Errors.Select(e => e.Description),
            };

        }
    }
}
