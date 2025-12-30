namespace JudgeAPI.Models.User
{
  public class UserDTO
  {
    public string Id {get; set;} = string.Empty;
    public string UserName {get; set;} = string.Empty;
    public string? FirstName {get; set;} = string.Empty;
    public string? LastName {get; set;} = string.Empty;
    public List<string> Roles {get; set;} = [];
    public bool IsActive {get; set;} = true;
  }
}
