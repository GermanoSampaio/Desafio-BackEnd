namespace MotoService.Domain.Interfaces
{
    public interface IRentalDomainService
    {
        Task ValidateRentalCreationAsync(string motorcycleId, string cnhType, DateTime startDate, DateTime endDate);
    }
}
