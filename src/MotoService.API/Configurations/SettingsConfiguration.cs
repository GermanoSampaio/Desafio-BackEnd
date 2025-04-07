using MotoService.Infrastructure.AWS.Settings;
using MotoService.Infrastructure.MessageBroker.Configurations;
using MotoService.Infrastructure.Persistence.Settings;

namespace MotoService.API.Configurations
{
    public static class SettingsConfiguration
    {
        public static void SetSettings(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<RabbitMqSettings>(options => configuration.GetSection("RabbitMqSettings").Bind(options));
            services.Configure<MongoDbSettings>(options => configuration.GetSection("MongoDbSettings").Bind(options));
            services.Configure<AWSSettings>(options => configuration.GetSection("AWSSettings").Bind(options));
        }
    }
}
