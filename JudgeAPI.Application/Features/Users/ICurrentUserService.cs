using JudgeAPI.Entities;

namespace JudgeAPI.Services.User
{
  public interface ICurrentUserService
  {
    IList<string> GetCurrentUserRole();
    Task<ApplicationUser> GetCurrentUserAsync();
    string? GetCurrentUserId();
    Task<ApplicationUser?> GetUserByIdAsync(string id);
    Task<IList<string>> GetUserRolesByIdAsync(ApplicationUser user);
  }
}
