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
        public async Task<IEnumerable<RentalPlan>> GetAllAsync()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }

        public async Task CreateAsync(RentalPlan rentalPlan)
        {
            await _collection.InsertOneAsync(rentalPlan);
        }
    }
}