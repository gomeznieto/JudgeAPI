using System.ComponentModel.DataAnnotations;

namespace JudgeAPI.Models.User
{
    public class UserUpdateDTO
    {
        [Required]
        public required string Id { get; set; }
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? University { get; set; }
    }
}
