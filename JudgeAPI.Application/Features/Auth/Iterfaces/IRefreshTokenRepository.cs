using JudgeAPI.Domain.Entities;

namespace JudgeAPI.Application.Features.Auth.Iterfaces
{
    public interface IRefreshTokenRepository 
    {
        Task<UserRefreshToken?> GetByUserIdAndHashAsync(string userId, string tokenHash);
        void Add(UserRefreshToken refreshToken);
        void Update(UserRefreshToken refreshToken);
    }
}
