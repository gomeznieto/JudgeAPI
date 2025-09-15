using JudgeAPI.Constants;
using Microsoft.AspNetCore.Identity;

namespace JudgeAPI.Infrastructure.Seed
{
    public static class RoleSeeder
    {
        public static async Task SeedRoleAsync(RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { Roles.Admin, Roles.Student, Roles.Moderator };

            foreach (var roleName in roleNames)
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }
    }
}
