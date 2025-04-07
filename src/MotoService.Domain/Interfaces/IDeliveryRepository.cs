using MotoService.Domain.Entities;

namespace MotoService.Domain.Interfaces
{
    public interface IDeliveryRepository
    {
        Task CreateAsync(Delivery delivery);
        Task<bool> ExistsByCNPJAsync(string cnpj);
        Task<bool> ExistsByCNHAsync(string cnhNumber);
        Task<Delivery?> GetByIdentifierAsync(string identifier);
        Task UpdateCNHImageUrlAsync(Delivery delivery);
        Task EnsureIndexesAsync();
    }
}
