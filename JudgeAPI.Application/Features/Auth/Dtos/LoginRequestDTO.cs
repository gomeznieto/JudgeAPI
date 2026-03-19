using System.ComponentModel.DataAnnotations;

namespace JudgeAPI.Application.Features
{
    public class LoginRequestDTO
    {
        [Required]
        public required string UserName { get; set; }
        [Required]
        public required string Password { get; set; }
    }

}
