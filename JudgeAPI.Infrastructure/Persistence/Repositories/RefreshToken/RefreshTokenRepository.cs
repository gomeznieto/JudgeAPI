using JudgeAPI.Application.Features.Auth.Iterfaces;
using JudgeAPI.Domain.Entities;
using JudgeAPI.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace JudgeAPI.Infrastructure.Persistence.Repositories.RefreshToken
{
    public class RefreshTokenRepository(AppDbContext dbContext) : IRefreshTokenRepository
    {
        private readonly AppDbContext _dbContext = dbContext;

        public async Task<UserRefreshToken?> GetByUserIdAndHashAsync(string userId, string tokenHash)
        {
            if (!Guid.TryParse(userId, out Guid parseUserId))
            {
                throw new ArgumentException("Invalid UserId format", nameof(userId));
            }

            var result = await _dbContext.UserRefreshTokens.FirstOrDefaultAsync( r => r.UserId == parseUserId && r.TokenHash == tokenHash);
            return result;
        } 

        public async Task AddAsync(UserRefreshToken refreshToken)
        {
            await _dbContext.UserRefreshTokens.AddAsync(refreshToken);
        }

        public async Task UpdateAsync(UserRefreshToken refreshToken)
        {
            _dbContext.UserRefreshTokens.Update(refreshToken);
        }
    }
}
