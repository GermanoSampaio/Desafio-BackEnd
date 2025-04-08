using Microsoft.AspNetCore.Mvc;
using MotoService.Application.DTOs;
using MotoService.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace MotoService.API.Controllers
{
    [ApiController]
    [Route("api/locacao")]
    [Tags("locações")]
    public class RentalController : ControllerBase
    {
        private readonly IRentalService _rentalService;

        public RentalController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

        [SwaggerOperation(Summary = "Consultar locação por id")]
        [HttpGet("{id}", Name = "GetRentalById")]
        public async Task<IActionResult> GetRentalByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return BadRequest(new ErrorDto("Request mal formada"));

            var rental = await _rentalService.GetRentalByIdAsync(id);
            if (rental == null)
                return NotFound(new ErrorDto("Locação não encontrada"));

            return Ok(rental);
        }

        [SwaggerOperation(Summary = "Informar data de devolução e calcular valor")]
        [HttpPut("{id}/devolucao")]
        public async Task<IActionResult> UpdateTerminalDateAsync(string id, [FromBody] TerminalDateRequestDTO terminalDate)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorDto("Dados inválidos.", details: ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

            var rental = await _rentalService.GetRentalByIdAsync(id);
            if (rental == null)
                return NotFound(new ErrorDto("Locação não encontrada."));
            
            await _rentalService.UpdateRentalAsync(rental, terminalDate.TerminalDate);
            return Ok(new { message = "Data de devolução informada com sucesso" });
            }

        [SwaggerOperation(Summary = "Alugar uma moto")]
        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] RentalRequestDTO createRental)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorDto("Dados inválidos.", details: ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

            var rental = await _rentalService.CreateRentalAsync(createRental);
            if (rental == null)
                return BadRequest(new ErrorDto("Não foi possível criar a locação."));

            return CreatedAtRoute("GetRentalById", new { id = rental.DeliveryId }, null);
        }
    }
}
