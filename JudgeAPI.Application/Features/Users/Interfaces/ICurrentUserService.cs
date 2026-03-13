namespace JudgeAPI.Application.Features;

public interface ICurrentUserService {
    IList<string> GetCurrentUserRole();
    Task<UserDTO> GetCurrentUserAsync();
    string? GetCurrentUserId();
    Task<UserDTO?> GetUserByIdAsync(string id);
    Task<IList<string>> GetUserRolesByIdAsync(UserDTO user);
}

