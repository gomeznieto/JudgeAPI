namespace JudgeAPI.Application.Features;
using System.ComponentModel.DataAnnotations;

public class UserUpdateRolesDTO {
    [Required]
    public List<string> Roles { get; set; } = new();
}
