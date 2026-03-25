using JudgeAPI.Application.Common.Configuration;
using JudgeAPI.Application.Features.Auth.Iterfaces;
using JudgeAPI.Infrastructure.Authentication;
using JudgeAPI.Infrastructure.Data;
using JudgeAPI.Infrastructure.Persistence.Cache;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace JudgeAPI.Infrastructure
{
    public static class DependencyInjection
    {

        public static IServiceCollection AddInfrastructure(this IServiceCollection service, IConfiguration configuration)
        {
            _ = service.AddPersistence(configuration);
            _ = service.AddAuth();
            _ = service.AddRedis(configuration);
            _ = service.Configure<SubmissionOptions>(configuration.GetSection(SubmissionOptions.SectionName));
            _ = service.Configure<CompilerOptions>(configuration.GetSection(CompilerOptions.SectionName));
            return service;
        }

        // DB CONTEXT
        private static IServiceCollection AddPersistence(this IServiceCollection service, IConfiguration configuration)
        {
            _ = service.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                        sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null))
                    );

            return service;
        }

        // TOKEN
        private static IServiceCollection AddAuth(this IServiceCollection service)
        {
            _ = service.AddScoped<ITokenService, TokenService>();

            return service;
        }

        // REDIS
        public static IServiceCollection AddRedis(this IServiceCollection services, IConfiguration configuration)
        {
            string? redisConnection = configuration["Redis:Connection"];

            _ = services.AddSingleton<IConnectionMultiplexer>(sp =>
                    ConnectionMultiplexer.Connect(redisConnection!));

            _ = services.AddScoped<ICacheService, RedisCacheService>();

            return services;
        }
    }
}
