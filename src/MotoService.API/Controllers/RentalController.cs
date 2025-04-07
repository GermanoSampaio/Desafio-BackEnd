using Microsoft.AspNetCore.Mvc;
using MotoService.Application.DTOs;
using MotoService.Application.Interfaces;

namespace MotoService.API.Controllers
{
    [ApiController]
    [Route("api/locacao")]
    public class RentalController : ControllerBase
    {
        private readonly IRentalService _rentalService;

        public RentalController(IRentalService rentalService)
        {
            _rentalService = rentalService;
        }

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

        [HttpPut("{id}/devolucao")]
        public async Task<IActionResult> UpdateTerminalDateAsync(string id, [FromBody] TerminalDateDTO terminalDate)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorDto("Dados inválidos.", details: ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

            var rental = await _rentalService.GetRentalByIdAsync(id);
            if (rental == null)
                return NotFound(new ErrorDto("Locação não encontrada."));

            rental.TerminalDate = terminalDate.TerminalDate;
            await _rentalService.UpdateRentalAsync(rental);
            return NoContent();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateRentalDTO createRental)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ErrorDto("Dados inválidos.", details: ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));

            var rental = await _rentalService.CreateRentalAsync(createRental);
            if (rental == null)
                return BadRequest(new ErrorDto("Não foi possível criar a locação."));

            return CreatedAtRoute("GetRentalById", new { id = rental.DeliveryId }, rental);
        }
    }
}
