using JudgeAPI.Models.Auth;
using JudgeAPI.Models.User;

namespace JudgeAPI.Services
{
    public interface IAuthService
    {
        Task<TokenResponse> LoginAsync(LoginRequest request);
        Task<TokenResponse> RegisterAsync(UserCreateDTO dto);
    }
}
