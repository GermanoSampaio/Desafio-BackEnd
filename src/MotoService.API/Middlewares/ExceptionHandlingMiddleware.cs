using System.Net;
using System.Text.Json;
using MotoService.Application.DTOs;
using MotoService.Domain.Exceptions;

namespace MotoService.API.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode;
            string message;
            object? errors = null;

            switch (exception)
            {
                case RentalNotFoundException rentalNotFound:
                    statusCode = HttpStatusCode.NotFound;
                    message = rentalNotFound.Message;
                    _logger.LogWarning(rentalNotFound, "Entidade não encontrada.");
                    break;

                case RentalPlanNotFoundException rentalNotFound:
                    statusCode = HttpStatusCode.NotFound;
                    message = rentalNotFound.Message;
                    _logger.LogWarning(rentalNotFound, "Entidade não encontrada.");
                    break;

                case MotorcycleNotFoundException motorcycleNotFound:
                    statusCode = HttpStatusCode.NotFound;
                    message = motorcycleNotFound.Message;
                    _logger.LogWarning(motorcycleNotFound, "Entidade não encontrada.");
                    break;

                case DeliveryNotFoundException deliveryNotFound:
                    statusCode = HttpStatusCode.NotFound;
                    message = deliveryNotFound.Message;
                    _logger.LogWarning(deliveryNotFound, "Entidade não encontrada.");
                    break;

                case DomainException domainEx:
                    statusCode = HttpStatusCode.BadRequest;
                    message = domainEx.Message;
                    errors = domainEx.Details;
                    _logger.LogWarning(domainEx, "Erro de domínio.");
                    break;
           
                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    message = "Erro interno no servidor.";
                    _logger.LogError(exception, "Erro inesperado.");
                    break;
            }

            var response = new ErrorDto(message, code: (int)statusCode, details: errors as List<string>);


            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            await context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}