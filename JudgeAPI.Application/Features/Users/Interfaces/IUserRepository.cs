using JudgeAPI.Application.Features.Users.Dtos;

namespace JudgeAPI.Application.Features.Users.Interfaces
{
    public interface IUserRepository
    {
        Task<List<UserDTO>> GetUsersPagedAsync(int page = 1, int totalPerPage = 20);
        Task<int> GetTotalUsersCountAsync();
    }
}
