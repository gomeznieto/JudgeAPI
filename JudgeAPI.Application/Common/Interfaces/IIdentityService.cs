namespace JudgeAPI.Application.Common;
using JudgeAPI.Application.Features;

public interface IIdentityService{
    // USERS
    Task<bool>CheckPasswordAsync(string email, string password);
    Task<UserDTO?> FindByNameAsync(string username);
    Task<UserDTO?> FindByIdAsync(string id);
    Task<IdentityResultDTO> CreateUserAsync(UserDTO user, string password);
    Task<IdentityResultDTO> UpdateUserAsync(UserUpdateDTO user, string id);
    Task<IdentityResultDTO>ChangePasswordAsync(string id, ChangePasswordDTO dto);

    // ROLES
    Task<bool> RoleExistsAsync(string role);
    Task<IdentityResultDTO> CreateRoleAsync(string role);
    Task AddRoleAsync(UserDTO user, string role);
    Task<IList<string>?> GetRoleAsync(UserDTO user);
    Task <IList<string>> GetAllRolesAsync();
    Task<bool> IsInRolAsync(UserDTO user, string role);
    Task<IdentityResultDTO> RemoveFromRoleAsync(UserDTO user, string role);
}
