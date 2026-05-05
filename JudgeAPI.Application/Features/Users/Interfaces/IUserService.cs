using JudgeAPI.Application.Common.Dtos;
using JudgeAPI.Application.Features.Users.Dtos;

namespace JudgeAPI.Application.Features.Users.Interfaces
{
    public interface IUserService
    {
        Task<IdentityResultDTO> ChangePasswordAsync(ChangePasswordDTO dto, string userId);
        Task<UserPrivateDTO> GetCurrentUser(string id);
        Task<UserBaseDTO> GetUserByIdAsync(string id, string currentId);
        Task<UserPrivateDTO> UpdateUser(UserUpdateDTO userUpdate, string currentId);
        Task<UserPublicDTO> UpdateUserRoles(Guid userId, UserUpdateRolesDTO userUpdateRoles);
        Task<RolesResponseDTO> GetRolesAsync();
        Task<UsersResponseDTO> GetUsersAsync(int page = 1, int totalPerPage = 20);
    }
}
