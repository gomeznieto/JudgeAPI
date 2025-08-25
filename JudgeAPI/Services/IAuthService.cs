using JudgeAPI.Models;

namespace JudgeAPI.Services
{
    public interface IAuthService
    {
        Task<UserResponseDTO> RegisterAsync(UserCreateDTO dto);
    }
}
