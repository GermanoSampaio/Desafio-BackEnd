using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MotoService.Domain.Enums;

namespace MotoService.Application.DTOs
{
    public class CreateDeliveryDTO
    {
        [JsonIgnore]
        public string Id { get; set; } = String.Empty;
        [Required(ErrorMessage = "O nome é obrigatório")]
        [JsonPropertyName("nome")]
        public string Name { get; set; } = string.Empty;
        [Required(ErrorMessage = "O CNPJ é obrigatório")]
        [JsonPropertyName("cnpj")]
        public string Cnpj { get; set; } = string.Empty;
        [Required(ErrorMessage = "A data de nascimento é obrigatória")]
        [JsonPropertyName("data_nascimento")]
        public DateTime BirthDate { get; set; }
        [Required(ErrorMessage = "O número da CNH é obrigatório")]
        [JsonPropertyName("numero_cnh")]
        public string CnhNumber { get; set; } = string.Empty;
        [Required(ErrorMessage = "O tipo de CNH é obrigatório")]
        [JsonPropertyName("tipo_cnh")]
        public CnhType CnhType { get; set; }
    }
}
