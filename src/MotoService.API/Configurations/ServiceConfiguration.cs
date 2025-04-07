using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MotoService.Application.Interfaces;
using MotoService.Application.Services;
using MotoService.Domain.Interfaces;
using MotoService.Domain.Repositories;
using MotoService.Infrastructure.BackgroundServices;
using MotoService.Infrastructure.MessageBroker;
using MotoService.Infrastructure.Persistence.Contexts;
using MotoService.Infrastructure.Persistence.Settings;
using MotoService.Infrastructure.Persistence;
using MotoService.Infrastructure.Persistence.Services;
using Amazon.S3;
using MotoService.Infrastructure.AWS;
using MotoService.Infrastructure.MessageBroker.Interfaces;

namespace MotoService.API.Configurations
{
    public static class ServiceConfiguration
    {
        public static void ConfigureServices(this WebApplicationBuilder builder)
        {
            var services = builder.Services;
            //AutoMapper
            services.AddAutoMapper(typeof(Program));

            // MongoDB
            services.Configure<MongoDbSettings>(builder.Configuration.GetSection(nameof(MongoDbSettings)));
            services.AddSingleton<IMongoClient>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                return new MongoClient(settings.ConnectionString);
            });
            services.AddSingleton<IMongoDatabase>(sp =>
            {
                var settings = sp.GetRequiredService<IOptions<MongoDbSettings>>().Value;
                var client = sp.GetRequiredService<IMongoClient>();
                return client.GetDatabase(settings.DatabaseName);
            });
            services.AddSingleton<MongoDbContext>();

            // Repositórios e Serviços
            services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
            services.AddScoped<IMotorcycleNotificationRepository, MotorcycleNotificationRepository>();
            services.AddScoped<IMotorcycleService, MotorcycleService>();
            services.AddScoped<IRentalRepository, RentalRepository>();
            services.AddScoped<IRentalService, RentalService>();
            services.AddSingleton<ISequenceGenerator, SequenceGenerator>();
            services.AddScoped<IRentalPlanRepository, RentalPlanRepository>();
            services.AddScoped<IDeliveryService, DeliveryService>();
            services.AddScoped<IFileStorageService, FileStorageService>();
            services.AddScoped<IDeliveryRepository, DeliveryRepository>();

            // RabbitMQ
            services.AddScoped<RabbitMQConsumer>();
            services.AddScoped<IRabbitMQPublisher, RabbitMQPublisher>();
            services.AddHostedService<RabbitMQListenerService>();

            //AWS
            services.AddAWSService<IAmazonS3>();
        }
    }
}
