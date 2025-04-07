using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MotoService.Domain.Entities;
using MotoService.Domain.Interfaces;
using MotoService.Domain.Repositories;
using MotoService.Infrastructure.Persistence.Contexts;

namespace MotoService.Infrastructure.Persistence
{
    public class DeliveryRepository : IDeliveryRepository
    {
        private readonly ILogger<DeliveryRepository> _logger;
        private readonly IMongoClient _mongoClient;
        private readonly IMongoCollection<Delivery> _collection;
        private readonly ISequenceGenerator _sequenceGenerator;
        public DeliveryRepository(ILogger<DeliveryRepository> logger, MongoDbContext context, ISequenceGenerator sequenceGenerator)
        {
            _logger = logger;
            _collection = context.Delivery;
            _sequenceGenerator = sequenceGenerator;
            _mongoClient = context.Client;
        }

        public async Task CreateAsync(Delivery delivery)
        {
            using var session = await _mongoClient.StartSessionAsync();
            try
            {
                long next = await _sequenceGenerator.GetNextSequenceValueAsync("delivery");
                delivery.SetIdentifier($"delivery{next:D6}");
                await _collection.InsertOneAsync(delivery);
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                _logger.LogError(ex, "Erro ao inserir entregador: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<bool> ExistsByCNPJAsync(string cnpj)
        {
            var filter = Builders<Delivery>.Filter.Eq(d => d.Cnpj, cnpj);
            return await _collection.Find(filter).AnyAsync();
        }

        public async Task<bool> ExistsByCNHAsync(string cnhNumber)
        {
            var filter = Builders<Delivery>.Filter.Eq(d => d.CnhNumber, cnhNumber);
            return await _collection.Find(filter).AnyAsync();
        }

        public async Task<Delivery?> GetByIdentifierAsync(string identifier)
        {
            var filter = Builders<Delivery>.Filter.Eq(d => d.Identifier, identifier);
            return await _collection.Find(filter).FirstOrDefaultAsync();
        }
        public async Task UpdateCNHImageUrlAsync(Delivery delivery)
        {
            var filter = Builders<Delivery>.Filter.Eq(d => d.Id, delivery.Id);
            var update = Builders<Delivery>.Update.Set(d => d.CnhImageURL, delivery.CnhImageURL);
            await _collection.UpdateOneAsync(filter, update);
        }
        public async Task EnsureIndexesAsync()
        {
            var indexBuilder = Builders<Delivery>.IndexKeys;

            var indexes = new List<CreateIndexModel<Delivery>>
            {
                new CreateIndexModel<Delivery>(indexBuilder.Ascending(x => x.Cnpj), new CreateIndexOptions { Unique = true }),
                new CreateIndexModel<Delivery>(indexBuilder.Ascending(x => x.CnhNumber), new CreateIndexOptions { Unique = true }),
                new CreateIndexModel<Delivery>(indexBuilder.Ascending(x => x.Identifier), new CreateIndexOptions { Unique = true })
            }; 
            
            await _collection.Indexes.CreateManyAsync(indexes);
            _logger.LogInformation("Índices do MongoDB criados com sucesso.");

        }
    }
}
