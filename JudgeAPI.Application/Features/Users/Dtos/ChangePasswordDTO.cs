using System.ComponentModel.DataAnnotations;

namespace JudgeAPI.Application.Features.Users.Dtos
{
    public class ChangePasswordDTO {
        [Required]
        public string OldPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;
    }

}
