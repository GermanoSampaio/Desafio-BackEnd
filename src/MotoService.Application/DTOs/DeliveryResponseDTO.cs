using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MotoService.Application.Converters;
using MotoService.Domain.Enums;

namespace MotoService.Application.DTOs
{
    public class DeliveryResponseDTO
    {
        [JsonIgnore]
        public string Id { get; set; } = String.Empty;
        [JsonPropertyName("identificador")]
        public string Identifier { get; set; } = String.Empty;
        [JsonPropertyName("nome")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; } = string.Empty;
        [JsonPropertyName("data_nascimento")]
        public DateOnly BirthDate { get; set; }
        [JsonPropertyName("numero_cnh")]
        public string CnhNumber { get; set; } = string.Empty;
        [JsonPropertyName("tipo_cnh")]
        [JsonConverter(typeof(CnhTypeJsonConverter))]
        public CnhType CnhType { get; set; }
        [JsonPropertyName("imagem_cnh")]
        public string CnhBase64String { get; set; } = String.Empty;
    }

    public record UploadResultDTO(string ImageUrl);
}
