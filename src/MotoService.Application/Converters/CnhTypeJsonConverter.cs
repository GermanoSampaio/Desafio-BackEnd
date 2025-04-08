using System.Text.Json.Serialization;
using System.Text.Json;
using MotoService.Domain.Enums;

namespace MotoService.Application.Converters
{
    public class CnhTypeJsonConverter : JsonConverter<CnhType>
    {
        public override CnhType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();

            return value switch
            {
                "A" => CnhType.A,
                "B" => CnhType.B,
                "AB" => CnhType.AB,
                _ => throw new JsonException($"Tipo de CNH inválido: {value}")
            };
        }

        public override void Write(Utf8JsonWriter writer, CnhType value, JsonSerializerOptions options)
        {
            var result = value switch
            {
                CnhType.A => "A",
                CnhType.B => "B",
                CnhType.AB => "AB",
                _ => throw new JsonException("Tipo de CNH inválido.")
            };

            writer.WriteStringValue(result);
        }
    }
}