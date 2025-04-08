using MongoDB.Bson.Serialization;
using MotoService.Infrastructure.Persistence.Serializers;

namespace MotoService.Infrastructure.Persistence
{
    public static class MongoDbInitializer
    {
        private static bool _isInitialized = false;
        private static readonly object _lock = new();

        public static void RegisterSerializers()
        {
            if (_isInitialized)
                return;

            lock (_lock)
            {
                if (_isInitialized)
                    return;

                BsonSerializer.RegisterSerializer(new CnhTypeBsonSerializer());

                _isInitialized = true;
            }
        }
    }
}