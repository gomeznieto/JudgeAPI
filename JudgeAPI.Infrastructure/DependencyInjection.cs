using JudgeAPI.Application.Common.Interfaces;
using JudgeAPI.Infrastructure.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace JudgeAPI.Infrastructure;

public static class DependencyInjection {

    public static IServiceCollection AddInfrastructure(this IServiceCollection service, IConfiguration configuration){
        service.AddPersistence(configuration);
        service.AddAuth(configuration);
        return service;
    }
    
    // DB CONTEXT
    private static IServiceCollection AddPersistence(this IServiceCollection service, IConfiguration configuration){
        service.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                    sql => sql.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null))
                );

        return service;
    }

    // TOKEN
    private static IServiceCollection AddAuth(this IServiceCollection service, IConfiguration configuration){

        service.AddScoped<ITokenService, TokenService>();

        return service;
    }
}
