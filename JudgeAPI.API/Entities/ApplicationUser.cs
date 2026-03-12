using Microsoft.AspNetCore.Identity;

namespace JudgeAPI.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? University { get; set; }
        public bool IsActive {get; set;} = true;
    }
}
