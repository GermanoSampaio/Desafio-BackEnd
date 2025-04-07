using System.Text.Json.Serialization;

namespace MotoService.Application.DTOs
{
    public class LicensePlateDTO
    {
        [JsonPropertyName("placa")]
        public string LicensePlate { get; set; } = String.Empty;
    }
}
