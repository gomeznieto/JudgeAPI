namespace JudgeAPI.Application.Common;

public interface ITokenService {
    public string GenerateToken(string userId, string Email, IList<string> roles);

}

