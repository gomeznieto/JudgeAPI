using System.ComponentModel.DataAnnotations;

namespace JudgeAPI.Models
{
    public class UserUpdateDTO
    {
        [Required]
        public required string Id;
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? University { get; set; }
    }
}
