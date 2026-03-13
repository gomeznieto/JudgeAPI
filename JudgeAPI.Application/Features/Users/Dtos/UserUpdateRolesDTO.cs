namespace JudgeAPI.Application.Features;
using System.ComponentModel.DataAnnotations;

public class UserUpdateRolesDTO {
    [Required]
    public string Id {get; set;} = string.Empty;
    [Required]
    public List<string> Roles { get; set; } = new();
}
