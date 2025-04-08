using Microsoft.AspNetCore.Http;
using MotoService.Application.DTOs;

namespace MotoService.Application.Interfaces
{
    public interface IDeliveryService
    {
        Task<DeliveryResponseDTO> RegisterAsync(DeliveryRequestDTO deliveryDto);
        Task<string> UploadCNHImageAsync(string identifier, string base64Image);
        Task<DeliveryResponseDTO?> GetByIdAsync(string identifier);
    }
}
