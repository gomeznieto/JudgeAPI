namespace JudgeAPI.Application.Common;

public interface ITokenService
{
    string GenerateToken(string userId, string Email, IList<string> roles);
}

