using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace MotoService.Application.DTOs
{
    public class LicensePlateDTO
    {
        [JsonPropertyName("placa")]
        public string LicensePlate { get; set; } = String.Empty;
    }
}
