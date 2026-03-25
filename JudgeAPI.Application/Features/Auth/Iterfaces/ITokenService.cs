namespace JudgeAPI.Application.Features.Auth.Iterfaces
{
    public interface ITokenService
    {
        string GenerateToken(string userId, string Email, IList<string> roles);
    }
}
