using JudgeAPI.Configuration;
using JudgeAPI.Data;
using JudgeAPI.Entities;
using JudgeAPI.Extensions;
using JudgeAPI.Infrastructure.Seed;
using JudgeAPI.Middleware;
using JudgeAPI.Services.Ath;
using JudgeAPI.Services.Execution;
using JudgeAPI.Services.Problem;
using JudgeAPI.Services.Submissions;
using JudgeAPI.Services.TestCase;
using JudgeAPI.Services.Token;
using JudgeAPI.Services.Unit;
using JudgeAPI.Services.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.Text;


// -- BUILDER -- //

var builder = WebApplication.CreateBuilder(args);

// API

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

// CORS
builder.Services.AddCorsPolicy();

// DB CONTEXT
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null)));

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
builder.Services.AddScoped<ITokenService, TokenService>();

// -- APP -- //

var app = builder.Build();

// MIGRATE AL INICIAR SERVICIO
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// INICIAMOS CARGA A LA DB
using (var scope = app.Services.CreateScope())
{
    // ROLES
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    // ADMIN
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    await UserSeeder.SeedAdminAsync(userManager);
}

app.UseMiddleware<ExceptionMiddleware>();
app.UserCorsPolicy();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
