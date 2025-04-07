using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MotoService.Infrastructure.MessageBroker;

namespace MotoService.Infrastructure.BackgroundServices
{
    public class RabbitMQListenerService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public RabbitMQListenerService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var consumer = scope.ServiceProvider.GetRequiredService<RabbitMQConsumer>();

            await consumer.StartConsumingAsync(stoppingToken);
        }
    }

}