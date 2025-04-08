using Microsoft.AspNetCore.Mvc;
using MotoService.Application.DTOs;
using MotoService.Application.Interfaces;
using MotoService.Domain.Exceptions;
using Swashbuckle.AspNetCore.Annotations;

namespace MotoService.API.Controllers
{
    [ApiController]
    [Route("api/entregador")]
    [Tags("entregadores")]
    public class DeliveryController : ControllerBase
    {
        private readonly IDeliveryService _deliveryService;
        private readonly ILogger<DeliveryController> _logger;


        public DeliveryController(IDeliveryService deliveryService, ILogger<DeliveryController> logger)
        {
            _deliveryService = deliveryService;
            _logger = logger;
        }

        [HttpPost("{id}/cnh")]
        [SwaggerOperation(Summary = "Enviar foto da CNH")]
        public async Task<IActionResult> UploadCNHAsync(string id, [FromBody] CnhRequestDTO request)
        {
            if (request?.CNHFile is null || request.CNHFile.Length == 0)
                return BadRequest(new ErrorDto("Arquivo da CNH é obrigatório."));

            try
            {
                var imageUrl = await _deliveryService.UploadCNHImageAsync(id, request.CNHFile);
                return CreatedAtRoute(nameof(GetByIdAsync), new { id = id }, null);
            }
            catch (InvalidFileFormatException ex)
            {
                _logger.LogError(ex, "Erro de formato de arquivo inválido.");
                return BadRequest("Formato de arquivo inválido.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao fazer upload da CNH.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Erro interno do servidor.");
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpGet("{id}", Name = "GetByIdAsync")]

        public async Task<IActionResult> GetByIdAsync(string id)
        {
            try
            {
                var delivery = await _deliveryService.GetByIdAsync(id);
                return Ok(delivery);
            }
            catch (DeliveryNotFoundException ex)
            {
                _logger.LogError(ex, "Entregador não encontrado.");
                return NotFound(new { Message = ex.Message });
            }
        }

        [SwaggerOperation(Summary = "Cadastrar entregador")]

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] DeliveryRequestDTO delivery)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(x => x.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                return BadRequest(new ErrorDto("Erro de validação", details: errors));
            }

            var result = await _deliveryService.RegisterAsync(delivery);

            return CreatedAtRoute(nameof(GetByIdAsync), new { id = result.Identifier }, null);
        }
    }
}
