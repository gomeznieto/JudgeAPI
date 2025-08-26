using JudgeAPI.Entities;

namespace JudgeAPI.Services
{
    public interface ITokenService
    {
        public string GenerateToken(ApplicationUser user, IList<string> roles);

    }
}
