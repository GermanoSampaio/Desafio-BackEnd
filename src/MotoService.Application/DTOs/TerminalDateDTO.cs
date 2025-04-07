using System.Text.Json.Serialization;

namespace MotoService.Application.DTOs
{
    public class TerminalDateDTO
    {
        [JsonPropertyName("data_devolucao")]
        public DateTime TerminalDate { get; set; }
    }
}
