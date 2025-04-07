using System.Threading.Tasks;
using MotoService.Domain.Entities;
using MotoService.Infrastructure.MessageBroker;

namespace MotoService.Infrastructure.MessageBroker.Interfaces
{
    public interface IRabbitMQPublisher
    {
        Task PublishMessageAsync(MotorcycleRegisteredEvent motorcycleEvent);
    }
}