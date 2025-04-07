using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MotoService.Domain.Entities;

namespace MotoService.Domain.Repositories
{
    public interface IMotorcycleNotificationRepository
    {
        Task StoreNotificationAsync(MotorcycleRegisteredEvent motorcycleEvent);
    }
}
