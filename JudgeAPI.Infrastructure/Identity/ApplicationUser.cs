using Microsoft.AspNetCore.Identity;
namespace JudgeAPI.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? University { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
