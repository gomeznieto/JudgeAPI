using JudgeAPI.Constants;
using Microsoft.AspNetCore.Identity;
using JudgeAPI.Entities;

namespace JudgeAPI.Infrastructure.Seed
{
    public static class UserSeeder
    {
        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
        {
            string? adminEmail = Environment.GetEnvironmentVariable("ADMIN_MAIL") ?? "admin@mail.com";
            string? adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "superfuerteysecreto123!";

            if (string.IsNullOrWhiteSpace(adminPassword) || string.IsNullOrEmpty(adminEmail))
                throw new Exception("ADMIN_PASSWORD o ADMIN_MAIL no está configurado en las variables de entorno.");

            var existingAdmin = await userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin is null)
            {
                var adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, Roles.Admin);
                }
                else
                {
                    throw new Exception($"Error creando Admin inicial: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }

        }
    }
}
