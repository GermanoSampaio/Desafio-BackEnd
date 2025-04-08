using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MotoService.Domain.Enums;

namespace MotoService.Infrastructure.Persistence.Serializers
{
    public class CnhTypeBsonSerializer : SerializerBase<CnhType>
    {
        public override CnhType Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            var value = context.Reader.ReadString();
            return value switch
            {
                "A" => CnhType.A,
                "B" => CnhType.B,
                "A-B" => CnhType.AB,
                _ => throw new FormatException($"Valor inválido para CNH Type: {value}")
            };
        }

        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, CnhType value)
        {
            var stringValue = value switch
            {
                CnhType.A => "A",
                CnhType.B => "B",
                CnhType.AB => "A-B",
                _ => throw new FormatException($"Tipo de CNH inválido: {value}")
            };
            context.Writer.WriteString(stringValue);
        }
    }
}