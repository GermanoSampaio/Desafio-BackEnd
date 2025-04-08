using System.Text.Json.Serialization;

namespace MotoService.Application.DTOs
{
    public class LicensePlateRequestDTO
    {
        [JsonPropertyName("placa")]
        public string LicensePlate { get; set; } = String.Empty;
    }
}
