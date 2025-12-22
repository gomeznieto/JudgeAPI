
using System.ComponentModel.DataAnnotations;

namespace JudgeAPI.Models.User{
  public class UserUpdateRolesDTO {
    [Required]
    public string Id {get; set;} = string.Empty;
    [Required]
    public List<string> Roles { get; set; } = new();
  }
}
