using DotNetEnv;
using JudgeAPI.API.Configuration;
using JudgeAPI.API.Extensions;
using JudgeAPI.API.Middleware;
using JudgeAPI.Application.Common.Interfaces;
using JudgeAPI.Application.Features.Auth.Iterfaces;
using JudgeAPI.Application.Features.Auth.Services;
using JudgeAPI.Application.Features.CodeExecutor.Interfaces;
using JudgeAPI.Application.Features.CodeExecutor.Services;
using JudgeAPI.Application.Features.Problems.Iterfaces;
using JudgeAPI.Application.Features.Problems.Services;
using JudgeAPI.Application.Features.SubmissionResults.Interfaces;
using JudgeAPI.Application.Features.Submissions.Interfaces;
using JudgeAPI.Application.Features.Submissions.Services;
using JudgeAPI.Application.Features.TestCases.Interfaces;
using JudgeAPI.Application.Features.TestCases.Services;
using JudgeAPI.Application.Features.Units.Interfaces;
using JudgeAPI.Application.Features.Units.Services;
using JudgeAPI.Application.Features.Users.Interfaces;
using JudgeAPI.Application.Features.Users.Services;
using JudgeAPI.Infrastructure;
using JudgeAPI.Infrastructure.Data;
using JudgeAPI.Infrastructure.Identity;
using JudgeAPI.Infrastructure.Persistence.Repositories.Problems;
using JudgeAPI.Infrastructure.Persistence.Repositories.SubmissionResults;
using JudgeAPI.Infrastructure.Persistence.Repositories.Submissions;
using JudgeAPI.Infrastructure.Persistence.Repositories.TestCases;
using JudgeAPI.Infrastructure.Persistence.Repositories.Users;
using JudgeAPI.Infrastructure.Persistence.Repositories.RefreshToken;
using JudgeAPI.Infrastructure.Seed;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

if (string.Equals(environment, "Development", StringComparison.OrdinalIgnoreCase))
{
    DotNetEnv.Env.Load("../.env.dev");
    var testDb = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
    var testAdmin = Environment.GetEnvironmentVariable("ADMIN_MAIL");
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("[DEBUG .ENV] Archivo .env.dev cargado.");
    Console.WriteLine($"[DEBUG .ENV] ADMIN_MAIL: {testAdmin}");
    Console.WriteLine($"[DEBUG .ENV] DefaultConnection: {testDb}");
    Console.ResetColor();
}
else
{
    DotNetEnv.Env.Load("../.env");
}

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddHealthChecks();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
builder.Services.AddCorsPolicy();
builder.Services.AddDataProtection();
builder.Services.AddHttpContextAccessor();

// DB CONTEXT, TOKEN, REDIS
builder.Services.AddInfrastructure(builder.Configuration);

// DB IDENTITY
builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT
builder.Services.AddJwtAuthentication(builder.Configuration);

// MAPPER
builder.Services.AddAutoMapper(typeof(Program).Assembly);

// RUNNER MODE
string mode = builder.Configuration["RunMode"] ?? "distributed";

if (mode.Equals("local", StringComparison.OrdinalIgnoreCase))
{
    _ = builder.Services.AddScoped<IAnalyzer, LocalAnalyzer>();
}
else
{
    _ = builder.Services.AddScoped<IAnalyzer, DistributedAnalyzer>();
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
builder.Services.AddTransient<ISubmissionResultsRepository, SubmissionResultRepository>();
builder.Services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddTransient<IUnitRepository, UnitRespository>();
// --------- APP --------- //
WebApplication app = builder.Build();

// MIGRATE AL INICIAR SERVICIO
using (IServiceScope scope = app.Services.CreateScope())
{
    AppDbContext db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    const int maxRetries = 20;
    const int delaySeconds = 5;

    int attempt = 0;
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
        catch (Exception ex)
        {
            Console.WriteLine($"[Error Real] {ex.Message}");
            if (ex.InnerException != null)
            {
                Console.WriteLine($"[Inner] {ex.InnerException.Message}");
            }

            if (attempt >= maxRetries)
            {
                throw;
            }

            Console.WriteLine(
                    $"[Start] La DB no está lista. Retry {attempt}/{maxRetries} en {delaySeconds}s"
                    );

            await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
        }
    }
}

// INICIAMOS CARGA A LA DB
using (IServiceScope scope = app.Services.CreateScope())
{
  // ROLES
  RoleManager<IdentityRole<Guid>> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
  await RoleSeeder.SeedRoleAsync(roleManager);

  // ADMIN
  UserManager<ApplicationUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
  await UserSeeder.SeedAdminAsync(userManager);
}

app.UseMiddleware<ExceptionMiddleware>();
app.UserCorsPolicy();
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/api/health");
app.MapControllers();

app.Run();

