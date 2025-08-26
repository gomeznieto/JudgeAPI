using JudgeAPI.Models;
using JudgeAPI.Models.Auth;

namespace JudgeAPI.Services
{
    public interface IAuthService
    {
        Task<TokenResponse> LoginAsync(LoginRequest request);
        Task<TokenResponse> RegisterAsync(UserCreateDTO dto);
    }
}
