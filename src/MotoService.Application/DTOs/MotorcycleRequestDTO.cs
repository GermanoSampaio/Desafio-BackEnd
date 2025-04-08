using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MotoService.Application.DTOs
{
    public class MotorcycleRequestDTO
    {
        [JsonIgnore]
        public string Id { get; set; } = String.Empty;
        [JsonPropertyName("identificador")]
        [Required(ErrorMessage = "O identificador é obrigatório")]
        [RegularExpression("^[0-9]{4}$")]
        public string Identifier { get; set; } = String.Empty;
        [JsonPropertyName("ano")]
        [Required(ErrorMessage = "O ano é obrigatório.")]
        [Range(1900, 2100, ErrorMessage = "O ano deve estar entre 1900 e 2100.")]
        public int Year { get; set; }
        [JsonPropertyName("modelo")]
        [Required(ErrorMessage = "O modelo é obrigatório.")]
        public string Model { get; set; } = String.Empty;
        [Required(ErrorMessage = "A placa é obrigatória.")]
        [RegularExpression("^[A-Z]{3}-[0-9]{4}$", ErrorMessage = "A placa deve estar no formato AAA-0000.")]
        [JsonPropertyName("placa")]
        public string LicensePlate { get; set; } = String.Empty;

    }
}
