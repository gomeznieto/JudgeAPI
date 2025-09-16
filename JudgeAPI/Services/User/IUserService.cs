using JudgeAPI.Models.User;

namespace JudgeAPI.Services.User
{
    public interface IUserService
    {
        Task<UserResponseDTO> GetUserByIdAsync(string id);
    }
}
