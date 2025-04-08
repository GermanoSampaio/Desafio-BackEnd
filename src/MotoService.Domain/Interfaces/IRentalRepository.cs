using MotoService.Domain.Entities;

namespace MotoService.Domain.Interfaces
{
    public interface IRentalRepository
    {
        Task<Rental?> GetRentalByIdAsync(string identifier);
        Task<Rental> CreateRentalAsync(Rental rentalDto);
        Task UpdateRentalAsync(Rental rentalDto, DateTime newTerminalDate);
        Task<List<Rental>> GetRentalsByMotoIdAsync(string motorcycleId);
    }
}
