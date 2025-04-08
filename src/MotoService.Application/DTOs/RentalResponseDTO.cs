using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using MotoService.Domain.Exceptions;

namespace MotoService.Application.DTOs
{
    public class RentalResponseDTO
    {
        [JsonPropertyName("identificador")]
        [Required(ErrorMessage = "O identificador é obrigatório.")]
        public string Identifier { get; set; } = String.Empty;
        [JsonPropertyName("entregador_id")]
        [Required(ErrorMessage = "O campo entregador_id é obrigatório.")]
        public string DeliveryId { get; set; } = String.Empty;
        [JsonPropertyName("moto_id")]
        [Required(ErrorMessage = "O campo moto_id é obrigatório.")]
        public string MotorcycleId { get; set; } = String.Empty;
        [JsonPropertyName("data_inicio")]
        [Required(ErrorMessage = "O data_inicio é obrigatório.")]
        public DateTime StartDate { get; set; }
        [JsonPropertyName("data_termino")]
        [Required(ErrorMessage = "O data_termino é obrigatório.")]
        public DateTime TerminalDate { get; set; }
        [JsonPropertyName("data_previsao_termino")]
        [Required(ErrorMessage = "O data_previsao_termino é obrigatório.")]
        public DateTime ExpectedTerminalDate { get; set; }
        [JsonPropertyName("plano")]
        [Required(ErrorMessage = "O plano é obrigatório.")]
        public int RentalPlan { get; set; }
        [JsonPropertyName("valor_diaria")]
        [Required(ErrorMessage = "O campo valor_diaria é obrigatório.")]
        public double DailyRate { get; set; }

    }
}