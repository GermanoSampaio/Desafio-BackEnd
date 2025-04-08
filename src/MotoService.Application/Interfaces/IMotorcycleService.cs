using MotoService.Application.DTOs;

namespace MotoService.Application.Interfaces
{
    public interface IMotorcycleService
    {
        Task<IEnumerable<MotorcycleResponseDTO>> GetAllAsync();
        Task<MotorcycleResponseDTO?> GetByIdAsync(string identifier);
        Task<string> CreateAsync(MotorcycleRequestDTO motorcycleDto);
        Task<bool> UpdateLicensePlateAsync(string id, string newPlate);
        Task DeleteAsync(string identifier);
    }
}
