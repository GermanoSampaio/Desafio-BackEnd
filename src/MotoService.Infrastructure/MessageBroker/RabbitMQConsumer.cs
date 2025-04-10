using System.Text;
using MotoService.Domain.Repositories;
using RabbitMQ.Client.Events;
using MotoService.Infrastructure.MessageBroker.Configurations;
using System.Text.Json;
using MotoService.Domain.Entities;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Microsoft.Extensions.Logging;

namespace MotoService.Infrastructure.MessageBroker
{
    public class RabbitMQConsumer
    {
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly RabbitMqSettings _settings;
        private readonly IMotorcycleNotificationRepository _repository;

        public RabbitMQConsumer(IOptions<RabbitMqSettings> settings, IMotorcycleNotificationRepository repository, ILogger<RabbitMQConsumer> logger)
        {
            _settings = settings.Value;
            _repository = repository;
            _logger = logger;
        }

        public async Task StartConsumingAsync(CancellationToken cancellationToken)
        {
            var(connection, channel) = await RabbitMqAsyncHelper.CreateConnectionAndChannelAsync(_settings.Hostname);

            await RabbitMqAsyncHelper.ConfigureQueueAndExchangeAsync(
                channel, _settings.Exchange, _settings.Queue, _settings.RoutingKey);

            var consumer = new AsyncEventingBasicConsumer(channel);

            consumer.ReceivedAsync += async (_, ea) =>
            {
                var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                MotorcycleRegisteredEvent? motorcycleEvent = null;


                try
                {
                    motorcycleEvent = JsonSerializer.Deserialize<MotorcycleRegisteredEvent>(message);

                    if (motorcycleEvent?.Year == 2024)
                    {
                        await _repository.StoreNotificationAsync(motorcycleEvent);

                        _logger.LogInformation("Moto 2024 cadastrada: ID {MotorcycleId}, Modelo: {Model}, Placa: {LicensePlate}, Ano:{Year}",
                        motorcycleEvent.Identifier, motorcycleEvent.Model, motorcycleEvent.LicensePlate, motorcycleEvent.Year);
                    }

                    await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erro ao processar mensagem: {Message}", message);
                    _logger.LogError(ex, "Erro ao processar mensagem. Conteúdo: {Message}, MotoId: {MotorcycleId}", message, motorcycleEvent?.Identifier ?? "desconhecido");
                }
                
            };

            await channel.BasicConsumeAsync(
               queue: _settings.Queue,
               autoAck: false,
               consumer: consumer
           );

            await Task.Delay(Timeout.Infinite, cancellationToken);
        }
    }
}