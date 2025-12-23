using JudgeAPI.Configuration;
using JudgeAPI.Data;
using JudgeAPI.Entities;
using JudgeAPI.Extensions;
using JudgeAPI.Infrastructure.Seed;
using JudgeAPI.Middleware;
using JudgeAPI.Services.Auth;
using JudgeAPI.Services.Execution;
using JudgeAPI.Services.Problem;
using JudgeAPI.Services.Submissions;
using JudgeAPI.Services.TestCase;
using JudgeAPI.Services.Token;
using JudgeAPI.Services.Unit;
using JudgeAPI.Services.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using DotNetEnv;

// -- ENV -- //
Env.Load();

// -- BUILDER -- //
var builder = WebApplication.CreateBuilder(args);

// --------- SERVICES --------- //

// HEALTHCHECKER
builder.Services.AddHealthChecks();

// API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();

// CORS
builder.Services.AddCorsPolicy();

// DB CONTEXT
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null))
);

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
var redisConnection = builder.Configuration["Redis:Connection"];
builder.Services.AddSingleton<IConnectionMultiplexer>(
    sp => ConnectionMultiplexer.Connect(redisConnection)
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

// --------- APP --------- //
var app = builder.Build();

// MIGRATE AL INICIAR SERVICIO
const int maxRetries = 20;
const int delaySeconds = 5;

using (var scope = app.Services.CreateScope())
{
  var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

  var attempt = 0;
  while(true){
    try{
      attempt++;
      db.Database.Migrate();
      break;
    } catch(Exception ex) {
      if(attempt >= maxRetries){
        throw;
      }

      Console.WriteLine($"[Start] La DB no está lista. Retry {attempt}/{maxRetries} en {delaySeconds}s");
    }

    await Task.Delay(TimeSpan.FromSeconds(delaySeconds));
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
