using MotoService.Domain.Entities;

namespace MotoService.Domain.Repositories
{
    public interface IMotorcycleNotificationRepository
    {
        Task StoreNotificationAsync(MotorcycleRegisteredEvent motorcycleEvent);
    }
}
