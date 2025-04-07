using Microsoft.AspNetCore.Mvc;
using MotoService.Application.DTOs;
using MotoService.Application.Interfaces;
using MotoService.Domain.Exceptions;
using Swashbuckle.AspNetCore.Annotations;

namespace MotoService.API.Controllers
{
    [ApiController]
    [Route("api/entregador")]
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
        [RequestSizeLimit(5_000_000)]
        [SwaggerOperation(Summary = "Upload da imagem da CNH de um entregador")]
        [SwaggerResponse(StatusCodes.Status200OK, "Upload realizado com sucesso")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Erro de validação")]
        [SwaggerResponse(StatusCodes.Status500InternalServerError, "Erro interno do servidor")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UploadCNHAsync(string id, [FromForm] IFormFile cnhFile)
        {
            if (cnhFile is null || cnhFile.Length == 0)
                return BadRequest(new ErrorDto("Arquivo da CNH é obrigatório."));
            try
            {
                var imageUrl = await _deliveryService.UploadCNHImageAsync(id, cnhFile);
                return Ok(new UploadResultDTO(imageUrl));
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

        [HttpPost]
        public async Task<IActionResult> CreateAsync([FromBody] CreateDeliveryDTO delivery)
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
            return CreatedAtAction(nameof(GetByIdAsync), new { id = result.Identifier }, result);
        }
    }
}
