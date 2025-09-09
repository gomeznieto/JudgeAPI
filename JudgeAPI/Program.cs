using JudgeAPI.Configuration;
using JudgeAPI.Data;
using JudgeAPI.Entities;
using JudgeAPI.Extensions;
using JudgeAPI.Middleware;
using JudgeAPI.Services.Ath;
using JudgeAPI.Services.Execution;
using JudgeAPI.Services.Problem;
using JudgeAPI.Services.Submissions;
using JudgeAPI.Services.TestCase;
using JudgeAPI.Services.Token;
using JudgeAPI.Services.Unit;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

// DB CONTEXT
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// DB IDENTITY
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddDataProtection();
builder.Services.AddHttpContextAccessor();

// JWT
builder.Services.AddJwtAuthentication(builder.Configuration);

// MAPPER
builder.Services.AddAutoMapper(typeof(Program));

// REDIS
builder.Services.AddSingleton<IConnectionMultiplexer>(
    sp => ConnectionMultiplexer.Connect("redis:6379")
);

// RUNNER MODE
var mode = builder.Configuration["RunMode"] ?? "distributed";

if (mode.Equals("local", StringComparison.OrdinalIgnoreCase))
    builder.Services.AddScoped<IAnalyzer, LocalAnalyzer>();
else
    builder.Services.AddScoped<IAnalyzer, DistributedAnalyzer>();

// RUNNER
builder.Services.AddSingleton(new RunnerConfig()
{
    Cpus = 1,
    MemoryMb = 256,
    PerTestTimeoutSeconds = 2,
    ImageName = "judge-cpp-runner"
});

// SERVICES
builder.Services.AddTransient<IUnitService, UnitService>();
builder.Services.AddTransient<IProblemService, ProblemService>();
builder.Services.AddTransient<IAuthService, AuthService>();
builder.Services.AddTransient<ISubmissionService, SubmissionService>();
builder.Services.AddTransient<ITestCaseService, TestCaseService>();
builder.Services.AddTransient<ICodeCompilerService, GppCodeCompilerService>();
builder.Services.AddTransient<ICodeExecutorService, BasicExecutorService>();
builder.Services.AddScoped<ITokenService, TokenService>();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
