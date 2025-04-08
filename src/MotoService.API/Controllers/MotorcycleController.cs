using System.Numerics;
using Microsoft.AspNetCore.Mvc;
using MotoService.Application.DTOs;
using MotoService.Application.Interfaces;
using MotoService.Domain.Entities;
using Swashbuckle.AspNetCore.Annotations;

namespace MotoService.API.Controllers
{
    [ApiController]
    [Route("api/motos")]
    [Tags("motos")]
    public class MotorcycleController : ControllerBase
    {
        private readonly IMotorcycleService _motorcycleService;

        public MotorcycleController(IMotorcycleService motorcycleService)
        {
            _motorcycleService = motorcycleService;
        }

        [SwaggerOperation(Summary = "Cadastrar uma nova moto")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] MotorcycleRequestDTO motorcycle)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorDto("Dados inválidos.", null, ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

            var createdMotorcycle = await _motorcycleService.CreateAsync(motorcycle);
            if (String.IsNullOrEmpty(createdMotorcycle))
                return BadRequest(new ErrorDto("Não foi possível criar a motocicleta."));

            return CreatedAtRoute("GetMotorcycleById", new { id = createdMotorcycle }, null);
        }

        [SwaggerOperation(Summary = "Consultar motos existentes")]
        [HttpGet]
        public async Task<IActionResult> GetAllAsync()
        {
            var motorcycles = await _motorcycleService.GetAllAsync();
            return Ok(motorcycles);
        }

        [SwaggerOperation(Summary = "Modificar a placa de uma moto")]
        [HttpPut("{id}/placa")]
        public async Task<IActionResult> UpdateLicensePlateAsync(string id, [FromBody] LicensePlateRequestDTO licensePlate)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorDto("Dados inválidos.", details: ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

            var updated = await _motorcycleService.UpdateLicensePlateAsync(id, licensePlate.LicensePlate);
            return Ok(new { message = "Placa modificada com sucesso" });
        }

        [SwaggerOperation(Summary = "Consultar motos existentes por id")]
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

        [SwaggerOperation(Summary = "Remover uma moto")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(string id)
        {
            var motorcycle = await _motorcycleService.GetByIdAsync(id);
           
            await _motorcycleService.DeleteAsync(id);
            return Ok();
        }

      
    }
}
