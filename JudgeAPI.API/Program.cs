using JudgeAPI.Infrastructure;
using JudgeAPI.Configuration;
using JudgeAPI.Extensions;
using JudgeAPI.Infrastructure.Seed;
using JudgeAPI.Middleware;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using DotNetEnv;
using JudgeAPI.Application.Features;
using JudgeAPI.Infrastructure.Identity;
using JudgeAPI.Application.Features.Submissions.Interfaces;
using JudgeAPI.Infrastructure.Persistence.Repositories.Submissions;
using JudgeAPI.Infrastructure.Persistence.Repositories.Problems;
using JudgeAPI.Application.Features.Auth.Services;
using JudgeAPI.Application.Features.Users.Services;
using JudgeAPI.Application.Features.Users.Interfaces;
using JudgeAPI.Application.Features.Auth.Iterfaces;
using JudgeAPI.Application.Common.Interfaces;
using JudgeAPI.Infrastructure.Persistence.Repositories.Users;
using JudgeAPI.Application.Features.Problems.Iterfaces;
using JudgeAPI.Application.Features.TestCases.Interfaces;
using JudgeAPI.Infrastructure.Persistence.TestCases;
Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddCorsPolicy();
builder.Services.AddDataProtection();
builder.Services.AddHttpContextAccessor();

// DB CONTEXT
builder.Services.AddInfrastructure(builder.Configuration);

// DB IDENTITY
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT
builder.Services.AddJwtAuthentication(builder.Configuration);

// MAPPER
builder.Services.AddAutoMapper(typeof(Program));

// REDIS
var redisConnection = builder.Configuration["Redis:Connection"];

foreach (var kv in builder.Configuration.AsEnumerable()){
    if (kv.Key.StartsWith("Redis")){
        Console.WriteLine($"{kv.Key} = {kv.Value}");
    }
}

builder.Services.AddSingleton<IConnectionMultiplexer>(
        sp => ConnectionMultiplexer.Connect(redisConnection)
        );

// RUNNER MODE
var mode = builder.Configuration["RunMode"] ?? "distributed";

if (mode.Equals("local", StringComparison.OrdinalIgnoreCase)){
    builder.Services.AddScoped<IAnalyzer, LocalAnalyzer>();
} else {
    builder.Services.AddScoped<IAnalyzer, DistributedAnalyzer>();
}

// SERVICIO QUE EJECUTA EL CPP EN LOCAL
builder.Services.AddSingleton(new RunnerConfig()
        {
        Cpus = 1,
        MemoryMb = 256,
        PerTestTimeoutSeconds = 2,
        ImageName = "judge-cpp-runner"
        });

// SERVICES PROJECT
builder.Services.AddTransient<ICurrentUserService, CurrentUserService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IUnitService, UnitService>();
builder.Services.AddTransient<IProblemService, ProblemService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<ISubmissionService, SubmissionService>();
builder.Services.AddTransient<ITestCaseService, TestCaseService>();
builder.Services.AddTransient<ICodeCompilerService, GppCodeCompilerService>();
builder.Services.AddTransient<ICodeExecutorService, BasicExecutorService>();
builder.Services.AddTransient<IIdentityService, IdentityService>();
builder.Services.AddTransient<ISubmissionRepository, SubmissionRepository>();
builder.Services.AddTransient<IProblemRepository, ProblemRepository>();
builder.Services.AddTransient<IUserRepository, UserRespository>();
builder.Services.AddTransient<ITestCaseRepository, TestCaseRepository>();
// --------- APP --------- //
var app = builder.Build();

// MIGRATE AL INICIAR SERVICIO
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    const int maxRetries = 20;
    const int delaySeconds = 5;

    var attempt = 0;
    while (true)
    {
        try
        {
            attempt++;

            await db.Database.OpenConnectionAsync();
            await db.Database.CloseConnectionAsync();

            db.Database.Migrate();
            break;
        }
        catch
        {
            if (attempt >= maxRetries)
                throw;

            Console.WriteLine(
                    $"[Start] La DB no está lista. Retry {attempt}/{maxRetries} en {delaySeconds}s"
                    );

            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        }
    }
}

// INICIAMOS CARGA A LA DB
using (var scope = app.Services.CreateScope())
{
    // ROLES
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedRoleAsync(roleManager);

    // ADMIN
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await UserSeeder.SeedAdminAsync(userManager);
}

app.UseMiddleware<ExceptionMiddleware>();
app.UserCorsPolicy();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/api/health");
app.MapControllers();

app.Run();
