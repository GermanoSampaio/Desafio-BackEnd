using System.Text.Json.Serialization;

namespace MotoService.Application.DTOs
{
    public class TerminalDateRequestDTO
    {
        [JsonPropertyName("data_devolucao")]
        public DateTime TerminalDate { get; set; }
    }
}
