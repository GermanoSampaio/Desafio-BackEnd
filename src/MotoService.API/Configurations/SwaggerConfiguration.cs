using Microsoft.OpenApi.Models;

namespace MotoService.API.Configurations
{
    public static class SwaggerConfiguration
    {
        public static void Configure(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.EnableAnnotations();
                options.SupportNonNullableReferenceTypes();
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "MotoService API",
                    Version = "v1"
                });
            });
        }
    }
}
