using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace MotoService.Application.DTOs
{
    public class CnhDTO
    {
        [Required]
        public IFormFile CNHFile { get; set; }
    }
}
