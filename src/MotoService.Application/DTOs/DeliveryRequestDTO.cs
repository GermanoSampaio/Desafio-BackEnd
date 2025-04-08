using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MotoService.Application.Converters;
using MotoService.Domain.Enums;

namespace MotoService.Application.DTOs
{
    public class DeliveryRequestDTO
    {
        [JsonIgnore]
        public string Id { get; set; } = String.Empty;
        
        [JsonPropertyName("identificador")]
        [Required(ErrorMessage = "O identificador é obrigatório")]
        [RegularExpression("^entregador[0-9]{4}$", ErrorMessage = "O identificador deve estar no formato 'entregador0000'.")]
        public string Identifier { get; set; } = String.Empty;
        
        [Required(ErrorMessage = "O nome é obrigatório")]
        [DefaultValue("João")]
        [JsonPropertyName("nome")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "O CNPJ é obrigatório")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "CNPJ deve conter exatamente 14 dígitos numéricos.")]
        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "A data de nascimento é obrigatória")]
        [JsonPropertyName("data_nascimento")]
        public DateOnly BirthDate { get; set; }
        
        [Required(ErrorMessage = "O número da CNH é obrigatório")]
        [JsonPropertyName("numero_cnh")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CNH deve conter exatamente 11 dígitos.")]
        public string CnhNumber { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "O tipo de CNH é obrigatório")]
        [DefaultValue("A")]
        [JsonPropertyName("tipo_cnh")]
        public string CnhType { get; set; } = string.Empty;

        [Required]
        [JsonPropertyName("imagem_cnh")]
        [DefaultValue("base64string")]
        public string CNHFileString { get; set; } = String.Empty;
    }
}
