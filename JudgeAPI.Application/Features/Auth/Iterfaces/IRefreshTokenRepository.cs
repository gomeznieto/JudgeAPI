using JudgeAPI.Domain.Entities;

namespace JudgeAPI.Application.Features.Auth.Iterfaces
{
    public interface IRefreshTokenRepository 
    {
        Task<UserRefreshToken?> GetByUserIdAndHashAsync(string userId, string tokenHash);
        Task AddAsync(UserRefreshToken refreshToken);
        Task UpdateAsync(UserRefreshToken refreshToken);
    }
}
