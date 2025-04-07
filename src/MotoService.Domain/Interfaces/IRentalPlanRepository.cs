using MotoService.Domain.Entities;

namespace MotoService.Domain.Interfaces
{
    public interface IRentalPlanRepository
    {
        Task<RentalPlan?> GetByDaysAsync(int days);
    }
}
