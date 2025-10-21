namespace JudgeAPI.Extensions
{
    public static class CorsExtension
    {
        private const string DefaultCorsPolicy = "DefaultCorsPolicy";

        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(opt =>
            {
                opt.AddPolicy(name: DefaultCorsPolicy, builder =>
                {
                    builder
                    .WithOrigins(
                        "http://localhost:5173"
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
            app.UseCors(DefaultCorsPolicy);

            return app;
        }
    }
}
