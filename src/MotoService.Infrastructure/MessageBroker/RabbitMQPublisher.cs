using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using MotoService.Domain.Entities;
using MotoService.Infrastructure.MessageBroker.Configurations;
using MotoService.Infrastructure.MessageBroker.Interfaces;
using RabbitMQ.Client;

namespace MotoService.Infrastructure.MessageBroker
{
    public class RabbitMQPublisher : IRabbitMQPublisher
    {
        private readonly RabbitMqSettings _settings;

        public RabbitMQPublisher(IOptions<RabbitMqSettings> settings)
        {
            _settings = settings.Value;
        }
        public async Task PublishMessageAsync(MotorcycleRegisteredEvent motorcycleEvent)
        {
            var (connection, channel) = await RabbitMqAsyncHelper.CreateConnectionAndChannelAsync(_settings.Hostname);

            await using (connection)
            await using (channel)
            {
                await RabbitMqAsyncHelper.ConfigureQueueAndExchangeAsync(channel, _settings.Exchange, _settings.Queue, _settings.RoutingKey);

                var message = JsonSerializer.Serialize(motorcycleEvent);
                var body = Encoding.UTF8.GetBytes(message);
                var properties = new BasicProperties() { DeliveryMode = DeliveryModes.Persistent };

                await channel.BasicPublishAsync(
                    exchange: _settings.Exchange,
                    routingKey: _settings.RoutingKey,
                    mandatory: false,
                    basicProperties: properties,
                    body: body.AsMemory()
                );
            }
        }
    }
}