using MotoService.Application.DTOs;

namespace MotoService.Application.Interfaces
{
    public interface IMotorcycleService
    {
        Task<IEnumerable<MotorcycleDTO>> GetAllAsync();
        Task<MotorcycleDTO?> GetByIdAsync(string identifier);
        Task<string> CreateAsync(CreateMotorcycleDTO motorcycleDto);
        Task<bool> UpdateLicensePlateAsync(string id, string newPlate);
        Task DeleteAsync(string identifier);
    }
}
