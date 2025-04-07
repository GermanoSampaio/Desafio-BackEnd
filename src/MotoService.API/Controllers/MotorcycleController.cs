using Microsoft.AspNetCore.Mvc;
using MotoService.Application.DTOs;
using MotoService.Application.Interfaces;

namespace MotoService.API.Controllers
{
    [ApiController]
    [Route("api/motos")]
    public class MotorcycleController : ControllerBase
    {
        private readonly IMotorcycleService _motorcycleService;

        public MotorcycleController(IMotorcycleService motorcycleService)
        {
            _motorcycleService = motorcycleService;
        }

        //[HttpGet]
        //public async Task<IActionResult> GetAllAsync([FromQuery, DefaultValue("CDX-0102"), SwaggerParameter("Placa", Required = false)] string placa)
        //{

        //    var motorcycles = await _motorcycleService.GetAllAsync();
        //    return Ok(motorcycles);
        //}
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var motorcycles = await _motorcycleService.GetAllAsync();
            return Ok(motorcycles);
        }

        [HttpGet("{id}", Name = "GetMotorcycleById")]
        public async Task<IActionResult> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new ErrorDto("Identificador inválido."));

            var motorcycle = await _motorcycleService.GetByIdAsync(id);
            if (motorcycle == null)
                return NotFound(new ErrorDto("Moto não encontrada."));

            return Ok(motorcycle);
        }

        [HttpPut("{id}/placa")]
        public async Task<IActionResult> UpdateLicensePlateAsync(string id, [FromBody] LicensePlateDTO licensePlate)
        {
            if (!ModelState.IsValid)
               return BadRequest(new ErrorDto("Dados inválidos.", details: ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

            var updated = await _motorcycleService.UpdateLicensePlateAsync(id, licensePlate.LicensePlate);
            return NoContent();
        }

            [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var motorcycle = await _motorcycleService.GetByIdAsync(id);
           
            await _motorcycleService.DeleteAsync(id);
            return Ok(new { message = "Moto excluída com sucesso." });
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateMotorcycleDTO motorcycle)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorDto("Dados inválidos.", null, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

            var createdMotorcycle = await _motorcycleService.CreateAsync(motorcycle);
            if (String.IsNullOrEmpty(createdMotorcycle))
                return BadRequest(new ErrorDto("Não foi possível criar a motocicleta."));

            return CreatedAtRoute("GetMotorcycleById", new { id = createdMotorcycle }, createdMotorcycle);
        }
    }
}
