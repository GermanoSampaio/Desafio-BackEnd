using AspNetCoreRateLimit;
using Microsoft.AspNetCore.RateLimiting;

namespace MotoService.API.Configurations
{
    public static class HeaderConfiguration
    {
        public static void Configure(WebApplicationBuilder builder)
        {
            builder.Services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true;
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    policy.WithOrigins("https://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });
            });

            builder.Services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter(policyName: "fixed", config =>
                {
                    config.Window = TimeSpan.FromMinutes(1);
                    config.PermitLimit = 10000;
                    config.QueueLimit = 0;
                });
            });

            builder.Services.AddMemoryCache();
            builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
            builder.Services.AddInMemoryRateLimiting();
            builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();
        }
    }
}