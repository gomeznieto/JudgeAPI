namespace JudgeAPI.Application.Features;
using System.ComponentModel.DataAnnotations;

public class LoginRequestDTO {
    [Required]
    public required string UserName { get; set; }
    [Required]
    public required string Password { get; set; }
}

