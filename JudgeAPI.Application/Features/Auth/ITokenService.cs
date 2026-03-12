namespace JudgeAPI.Application.Common.Interfaces;

public interface ITokenService
{
    public string GenerateToken(string userId, string Email, IList<string> roles);

}

