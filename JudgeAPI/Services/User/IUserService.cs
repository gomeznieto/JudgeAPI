using JudgeAPI.Models.User;
using Microsoft.AspNetCore.Identity;

namespace JudgeAPI.Services.User
{
    public interface IUserService
    {
        Task<IdentityResult> ChangePasswordAsync(ChangePasswordDTO changePasswordDTO);
        Task<UserBaseDTO> GetUserByIdAsync(string id);
        Task<UserPrivateDTO> UpdateUser(UserUpdateDTO userUpdate);
    }
}
