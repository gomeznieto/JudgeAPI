using StackExchange.Redis;

namespace JudgeAPI.Infrastructure.Persistence.Cache
{

    public class RedisCacheService(IConnectionMultiplexer redis) : ICacheService
    {
        private readonly IDatabase _db = redis.GetDatabase();

        public async Task ListRightPushAsync(string type, string job)
        {
            _ = await _db.ListRightPushAsync("submissions", job);
        }
    }
}
