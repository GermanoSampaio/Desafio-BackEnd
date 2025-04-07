using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Mongo2Go;
using Moq;
using MotoService.Domain.Interfaces;
using MotoService.Domain.Repositories;
using MotoService.Infrastructure.MessageBroker.Interfaces;
using MotoService.Infrastructure.Persistence.Settings;

namespace tests.Integration.Factories
{
    public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private MongoDbRunner _mongoRunner;

        public Mock<IRentalRepository> RentalRepositoryMock { get; } = new();
        public Mock<IMotorcycleRepository> MotorcycleRepositoryMock { get; } = new();
        public Mock<IFileStorageService> FileStorageServiceMock { get; } = new();
        public Mock<IRabbitMQPublisher> RabbitMQPublisherMock { get; } = new();

        protected override IHost CreateHost(IHostBuilder builder)
        {
            builder.UseEnvironment("Testing");

            _mongoRunner = MongoDbRunner.Start(singleNodeReplSet: true); // replica set para transações

            builder.ConfigureServices(services =>
            {
                // Substitui settings para usar Mongo2Go
                var mongoSettingsDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(MongoDbSettings));
                if (mongoSettingsDescriptor != null) services.Remove(mongoSettingsDescriptor);

                services.AddSingleton(new MongoDbSettings
                {
                    ConnectionString = _mongoRunner.ConnectionString,
                    DatabaseName = "TestDb"
                });

                // Remove serviços reais
                RemoveServiceIfExists<IRentalRepository>(services);
                RemoveServiceIfExists<IMotorcycleRepository>(services);
                RemoveServiceIfExists<IFileStorageService>(services);
                RemoveServiceIfExists<IRabbitMQPublisher>(services);

                // Injeta mocks
                services.AddSingleton(RentalRepositoryMock.Object);
                services.AddSingleton(MotorcycleRepositoryMock.Object);
                services.AddSingleton(FileStorageServiceMock.Object);
                services.AddSingleton(RabbitMQPublisherMock.Object);
            });

            return base.CreateHost(builder);
        }

        private void RemoveServiceIfExists<T>(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(T));
            if (descriptor != null)
                services.Remove(descriptor);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            _mongoRunner?.Dispose();
        }
    }
}
