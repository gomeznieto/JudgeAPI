using JudgeAPI.Entities;

namespace JudgeAPI.Services.User
{
    public interface ICurrentUserService
    {
        IList<string> GetCurrentUserRole();
        Task<ApplicationUser?> GetCurrentUserAsync();
        string? GetCurrentUserId();
    }
}
