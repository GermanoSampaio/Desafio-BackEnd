using Microsoft.AspNetCore.Http;
using MotoService.Application.DTOs;

namespace MotoService.Application.Interfaces
{
    public interface IDeliveryService
    {
        Task<DeliveryDTO> RegisterAsync(CreateDeliveryDTO deliveryDto);
        Task<string> UploadCNHImageAsync(string identifier, IFormFile cnhFile);
        Task<DeliveryDTO?> GetByIdAsync(string identifier);
    }
}
