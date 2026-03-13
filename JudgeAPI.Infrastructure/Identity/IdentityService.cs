namespace JudgeAPI.Infrastructure.Identity;

using System.Collections.Generic;
using System.Threading.Tasks;
using JudgeAPI.Application.Common;
using JudgeAPI.Application.Features;
using Microsoft.AspNetCore.Identity;

public sealed class IdentityService : IIdentityService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public IdentityService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

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
            Successed = result.Succeeded,
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
            Successed = result.Succeeded,
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
}
