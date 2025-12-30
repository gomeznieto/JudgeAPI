using JudgeAPI.Models.User;
using Microsoft.AspNetCore.Identity;

namespace JudgeAPI.Services.User
{
    public interface IUserService
    {
        Task<IdentityResult> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO);
        Task<UserPrivateDTO> GetCurrectUser();
        Task<UserBaseDTO> GetUserByIdAsync(string id);
        Task<UserPrivateDTO> UpdateUser(UserUpdateDTO userUpdate);
        Task<UserPublicDTO> UpdateUserRoles(UserUpdateRolesDTO userUpdateRoles);
        Task<RolesResponseDTO> GetRolesAsync();
        Task<UsersResponseDTO> GetUsersAsync(int page = 1, int totalPerPage = 20);
    }
}
