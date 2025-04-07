
using MotoService.Domain.Entities;
using MotoService.Infrastructure.MessageBroker.Interfaces;
using System.Text.Json;

namespace MotoService.Tests.Integration.Mocks
{
    internal class RabbitMqPublisherMock : IRabbitMQPublisher
    {
        public List<string> PublishedMessages = new();

        public Task PublishMessageAsync(MotorcycleRegisteredEvent motorcycleEvent)
        {
            PublishedMessages.Add(JsonSerializer.Serialize(motorcycleEvent));
            return Task.CompletedTask;
        }
    }
}
