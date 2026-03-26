using Microsoft.AspNetCore.Identity;
using JudgeAPI.Domain.Constants;

namespace JudgeAPI.Infrastructure.Seed
{
    public static class RoleSeeder
    {
        public static async Task SeedRoleAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = [Roles.Admin, Roles.Student, Roles.Moderator];

            foreach (string roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    _ = await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
