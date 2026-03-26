using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using JudgeAPI.Infrastructure.Data;
using JudgeAPI.API.Configuration;
using RunnerApp.Services;

var builder = Host.CreateDefaultBuilder(args)
  .ConfigureServices((ctx, services) =>
      {
            var config = ctx.Configuration;

            _ = services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

            _ = services.AddSingleton<IConnectionMultiplexer>(
                _ => ConnectionMultiplexer.Connect(config["Redis:Connection"] ?? "redis:6379"));

            // RunnerConfig desde sección "Runner"
            _ = services.Configure<RunnerConfig>(config.GetSection("Runner"));
            _ = services.AddSingleton(resolver =>
                resolver.GetRequiredService<Microsoft.Extensions.Options.IOptions<RunnerConfig>>().Value);

            _ = services.AddHostedService<RunnerWorker>();

      });

await builder.Build().RunAsync();
