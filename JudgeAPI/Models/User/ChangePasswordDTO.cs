using System.ComponentModel.DataAnnotations;

namespace JudgeAPI.Models.User
{
    public class ChangePasswordDTO
    {
        [Required]
        public string OldPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(6)]
        public string NewPassword { get; set; } = string.Empty;

        [Compare("NewPassword", ErrorMessage = "Las constraseñas no coincden")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
