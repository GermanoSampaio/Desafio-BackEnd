using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MotoService.Domain.Entities;
using MotoService.Infrastructure.Persistence.Settings;

namespace MotoService.Infrastructure.Persistence.Contexts
{
    public class MongoDbContext
    {
        private readonly IMongoClient _client;
        private readonly IMongoDatabase _database;
        private readonly MongoDbSettings _settings;

        public MongoDbContext(IConfiguration configuration, IOptions<MongoDbSettings> settings)
        {
            _settings = settings.Value;
            _client = new MongoClient(_settings.ConnectionString);
            _database = _client.GetDatabase(_settings.DatabaseName);
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }

        public IMongoClient Client => _client;

        public IMongoCollection<Motorcycle> Motorcycles => _database.GetCollection<Motorcycle>("motorcycles");
        public IMongoCollection<MotorcycleRegisteredEvent> MotorcycleNotifications => _database.GetCollection<MotorcycleRegisteredEvent>("motorcycle_notifications");
        public IMongoCollection<RentalPlan> RentalPlan => _database.GetCollection<RentalPlan>("rental_plans");
        public IMongoCollection<Rental> Rental => _database.GetCollection<Rental>("rental");
        public IMongoCollection<Delivery> Delivery => _database.GetCollection<Delivery>("delivery");
    }
}
