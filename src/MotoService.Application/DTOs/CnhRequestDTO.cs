using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace MotoService.Application.DTOs
{
    public class CnhRequestDTO
    {
        [Required]
        [JsonPropertyName("imagem_cnh")]
        [DefaultValue("base64string")]
        public string CNHFile { get; set; } = String.Empty;
    }
}
