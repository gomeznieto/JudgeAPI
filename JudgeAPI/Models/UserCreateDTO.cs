using System.ComponentModel.DataAnnotations;

namespace JudgeAPI.Models
{
    public class UserCreateDTO
    {
        [Required]
        public required string Username { get; set; }
        [Required]
        public required string Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName{ get; set; }
        public string? Email { get; set; }
        public string? Universidad { get; set; }
    }
}
