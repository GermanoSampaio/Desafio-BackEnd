using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MotoService.Domain.Enums;

namespace MotoService.Application.DTOs
{
    public class DeliveryDTO
    {
        [JsonIgnore]
        public string Id { get; set; } = String.Empty;
        [JsonPropertyName("identificador")]
        [Required(ErrorMessage = "O identificador é obrigatório.")]
        public string Identifier { get; set; } = String.Empty;
        [JsonPropertyName("nome")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; } = string.Empty;
        [JsonPropertyName("data_nascimento")]
        public DateTime BirthDate { get; set; }
        [JsonPropertyName("numero_cnh")]
        public string CnhNumber { get; set; } = string.Empty;
        [JsonPropertyName("tipo_cnh")]
        public CnhType CnhType { get; set; }
        [JsonPropertyName("imagem_cnh")]
        public string CnhImageURL { get; set; } = string.Empty;
    }

    public record UploadResultDTO(string ImageUrl);
}
