namespace JudgeAPI.Application.Common;
using JudgeAPI.Application.Features;

public interface IIdentityService{
    // USERS
    Task<bool>CheckPasswordAsync(string email, string password);
    Task<UserDTO?> FindByNameAsync(string username);
    Task<IdentityResultDTO> CreateUserAsync(UserDTO user, string password);

    // ROLES
    Task<bool> RoleExistsAsync(string role);
    Task<IdentityResultDTO> CreateRoleAsync(string role);
    Task AddRoleAsync(UserDTO user, string role);
    Task<IList<string>?> GetRoleAsync(UserDTO user);
}
