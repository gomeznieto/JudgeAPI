using JudgeAPI.Entities;
using JudgeAPI.Models.Auth;

namespace JudgeAPI.Services.User
{
    public interface ICurrentUserService
    {
        IList<string> GetCurrentUserRole();
        Task<TokenResponse> GetCurrentUserAsync();
        string? GetCurrentUserId();
    }
}
