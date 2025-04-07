using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace MotoService.Domain.Entities
{
    public class MotorcycleRegisteredEvent
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [BsonElement("_id")]
        public string Id { get; set; } = string.Empty;
        [JsonPropertyName("identificador")]
        public string Identifier { get; set; } = string.Empty;

        [JsonPropertyName("ano")]
        public int Year { get; set; }

        [JsonPropertyName("modelo")]
        public string Model { get; set; } = string.Empty;

        [JsonPropertyName("placa")]
        public string LicensePlate { get; set; } = string.Empty;
        [JsonPropertyName("data-registro")]
        public DateOnly DateRegistered { get; set; }
    }
}
