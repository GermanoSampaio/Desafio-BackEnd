using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MotoService.Domain.Entities;
using MotoService.Domain.Repositories;
using MotoService.Infrastructure.Persistence.Contexts;

namespace MotoService.Infrastructure.Persistence
{
    public class MotorcycleRepository : IMotorcycleRepository
    {
        private readonly IMongoClient _mongoClient;
        private readonly IMongoCollection<Motorcycle> _motorcycles;
        private readonly ILogger<MotorcycleRepository> _logger;
        private readonly ISequenceGenerator _sequenceGenerator;

        public MotorcycleRepository(MongoDbContext context, ILogger<MotorcycleRepository> logger, ISequenceGenerator sequenceGenerator)
        {
            _logger = logger;
            _motorcycles = context.Motorcycles;
            _sequenceGenerator = sequenceGenerator;
            _mongoClient = context.Client;

            var indexKeys = Builders<Motorcycle>.IndexKeys.Ascending(m => m.LicensePlate);
            var indexOptions = new CreateIndexOptions { Unique = true };
            var indexModel = new CreateIndexModel<Motorcycle>(indexKeys, indexOptions);

            _motorcycles.Indexes.CreateOne(indexModel);
            _sequenceGenerator = sequenceGenerator;
        }

        public async Task<List<Motorcycle>> GetAllAsync()
        {
            return await _motorcycles.Find(_ => true).ToListAsync();
        }

        public async Task<Motorcycle?> GetByIdAsync(string id)
        {
            return await _motorcycles.Find(m => m.Identifier == id).FirstOrDefaultAsync();
        }

        public async Task<string> CreateAsync(Motorcycle motorcycle)
        {
            try
            {
                long next = await _sequenceGenerator.GetNextSequenceValueAsync("moto");
                motorcycle.Identifier = $"moto{next:D6}";

                await _motorcycles.InsertOneAsync(motorcycle);
                return motorcycle.Identifier;
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                _logger.LogError(ex, "Erro ao inserir entregador: {Message}", ex.Message);
                return String.Empty;
            }
        }

        public async Task UpdateAsync(Motorcycle motorcycle)
        {
            var filter = Builders<Motorcycle>.Filter.Eq(m => m.Identifier, motorcycle.Identifier);
            await _motorcycles.ReplaceOneAsync(filter, motorcycle);
        }

        public async Task DeleteAsync(string id)
        {
            await _motorcycles.DeleteOneAsync(m => m.Identifier == id);
        }
    }
}