using MongoDB.Driver;
using MotoService.Domain.Entities;
using MotoService.Domain.Interfaces;
using MotoService.Infrastructure.Persistence.Contexts;

namespace MotoService.Infrastructure.Persistence
{
    public class RentalPlanRepository : IRentalPlanRepository
    {
        private readonly IMongoCollection<RentalPlan> _collection;
        
        public RentalPlanRepository(MongoDbContext context)
        {
            _collection = context.RentalPlan;

        }

        public async Task<RentalPlan?> GetByDaysAsync(int days)
        {
            return await _collection.Find(p => p.Days == days).FirstOrDefaultAsync();
        }
    }
}