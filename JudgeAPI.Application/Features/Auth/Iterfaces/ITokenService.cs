using System.Security.Claims;

namespace JudgeAPI.Application.Features.Auth.Iterfaces
{
    public interface ITokenService
    {
        string GenerateToken(string userId, string userName, IList<string> roles);
        string GenerateRefreshToken();
        string GetHashToken(string token);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
    }
}
