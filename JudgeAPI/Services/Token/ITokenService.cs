using JudgeAPI.Entities;

namespace JudgeAPI.Services.Token
{
    public interface ITokenService
    {
        public string GenerateToken(ApplicationUser user, IList<string> roles);

    }
}
