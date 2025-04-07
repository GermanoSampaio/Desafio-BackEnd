using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using MotoService.Domain.Entities;
using MotoService.Domain.Repositories;
using MotoService.Infrastructure.Persistence.Contexts;

namespace MotoService.Infrastructure.Persistence
{
    public class MotorcycleNotificationRepository : IMotorcycleNotificationRepository
    {
        private readonly IMongoCollection<MotorcycleRegisteredEvent> _collection;

        public MotorcycleNotificationRepository(MongoDbContext context)
        {
            _collection = context.MotorcycleNotifications;
        }

        public async Task StoreNotificationAsync(MotorcycleRegisteredEvent motorcycleEvent)
        {
            await _collection.InsertOneAsync(motorcycleEvent);
        }
    }
}