using JudgeAPI.Data;
using StackExchange.Redis;
using Microsoft.EntityFrameworkCore;
using JudgeAPI.Configuration;
using JudgeAPI.Services.Execution;


var builder = Host.CreateDefaultBuilder(args)
    .ConfigureServices((ctx, services) =>
    {
        var config = ctx.Configuration;

        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(config.GetConnectionString("DefaultConnection")));

        services.AddSingleton<IConnectionMultiplexer>(
            _ => ConnectionMultiplexer.Connect(config["Redis:Connection"] ?? "redis:6379"));

        // RunnerConfig desde sección "Runner"
        services.Configure<RunnerConfig>(config.GetSection("Runner"));
        services.AddSingleton(resolver =>
            resolver.GetRequiredService<Microsoft.Extensions.Options.IOptions<RunnerConfig>>().Value);

        Console.WriteLine("🚀 RunnerApp iniciado...");

        services.AddHostedService<RunnerWorker>();

    });

await builder.Build().RunAsync();
