using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace MotoService.Application.DTOs
{
    public class CnhDTO
    {
        [Required]
        public IFormFile CNHFile { get; set; }
    }
}
