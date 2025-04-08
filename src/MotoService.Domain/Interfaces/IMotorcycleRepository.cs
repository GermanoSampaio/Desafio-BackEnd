using MotoService.Domain.Entities;

namespace MotoService.Domain.Repositories
{
    public interface IMotorcycleRepository
    {
        Task<Motorcycle?> GetByIdAsync(string id);
        Task<List<Motorcycle>> GetAllAsync();
        Task<bool> CreateAsync(Motorcycle motorcycle);
        Task UpdateAsync(Motorcycle motorcycle);
        Task DeleteAsync(string id);
    }
}
