using System.ComponentModel.DataAnnotations;

namespace JudgeAPI.Models.Auth
{
    public class LoginRequest
    {
        [Required]
        public required string UserName { get; set; }
        [Required]
        public required string Password { get; set; }
    }
}
