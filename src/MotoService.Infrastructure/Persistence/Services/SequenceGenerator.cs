using MongoDB.Bson;
using MongoDB.Driver;
using MotoService.Domain.Repositories;

namespace MotoService.Infrastructure.Persistence.Services
{
    public class SequenceGenerator : ISequenceGenerator
    {
        private readonly IMongoDatabase _database;

        public SequenceGenerator(IMongoDatabase database)
        {
            _database = database;
        }

        public async Task<long> GetNextSequenceValueAsync(string collectionName)
        {
            var counters = _database.GetCollection<BsonDocument>("counters");

            var filter = Builders<BsonDocument>.Filter.Eq("_id", collectionName);
            var update = Builders<BsonDocument>.Update.Inc("sequence_value", 1);

            var options = new FindOneAndUpdateOptions<BsonDocument>
            {
                IsUpsert = true,
                ReturnDocument = ReturnDocument.After
            };

            var result = await counters.FindOneAndUpdateAsync(filter, update, options);

            return result["sequence_value"].AsInt64;
        }
    }
}