using MotoService.Domain.Entities;

namespace MotoService.Infrastructure.MessageBroker.Interfaces
{
    public interface IRabbitMQPublisher
    {
        Task PublishMessageAsync(MotorcycleRegisteredEvent motorcycleEvent);
    }
}