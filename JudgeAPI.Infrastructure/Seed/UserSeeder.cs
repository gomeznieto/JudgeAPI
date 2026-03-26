using Microsoft.AspNetCore.Identity;
using JudgeAPI.Infrastructure.Identity;
using JudgeAPI.Domain.Constants;

namespace JudgeAPI.Infrastructure.Seed
{
    public static class UserSeeder
    {
        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
        {
            string? adminEmail = Environment.GetEnvironmentVariable("ADMIN_MAIL") ?? "admin@mail.com";
            string? adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "superfuerteysecreto123!";

            if (string.IsNullOrWhiteSpace(adminPassword) || string.IsNullOrEmpty(adminEmail))
            {
                throw new Exception("ADMIN_PASSWORD o ADMIN_MAIL no está configurado en las variables de entorno.");
            }

            ApplicationUser? existingAdmin = await userManager.FindByEmailAsync(adminEmail);

            if (existingAdmin is null)
            {
                ApplicationUser adminUser = new()
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                IdentityResult result = await userManager.CreateAsync(adminUser, adminPassword);

                _ = result.Succeeded
                    ? await userManager.AddToRoleAsync(adminUser, Roles.Admin)
                    : throw new Exception($"Error creando Admin inicial: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

        }
    }
}
