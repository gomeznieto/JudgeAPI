namespace JudgeAPI.API.Extensions
{
    public static class CorsExtension
    {
        private const string DefaultCorsPolicy = "DefaultCorsPolicy";

        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            _ = services.AddCors(static opt =>
                    {
                        opt.AddPolicy(name: DefaultCorsPolicy, static builder =>
                                {
                                    _ = builder
                                .WithOrigins(
                                        "http://localhost:5174"
                                        // URL DE PRODUCCION
                                        )
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .AllowCredentials();
                                });
                    });

            return services;
        }

        public static IApplicationBuilder UserCorsPolicy(this IApplicationBuilder app)
        {
            _ = app.UseCors(DefaultCorsPolicy);

            return app;
        }
    }
}
