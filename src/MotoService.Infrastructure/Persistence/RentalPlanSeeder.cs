using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MotoService.Domain.Entities;
using MotoService.Domain.Interfaces;

namespace MotoService.Infrastructure.Persistence
{
    public static class RentalPlanSeeder
    {
        public static async Task SeedAsync(IRentalPlanRepository rentalPlanRepository, ILogger logger, IConfiguration configuration)
        {
            var existing = await rentalPlanRepository.GetAllAsync();
            if (existing.Any()) return;
        
            var plans = new List<RentalPlan>
            {
                new RentalPlan(7, 30.0),
                new RentalPlan(15, 28.0),
                new RentalPlan(30, 22.0),
                new RentalPlan(45, 20.0),
                new RentalPlan(50, 18.0)
            };

            foreach (var plan in plans)
            {
                await rentalPlanRepository.CreateAsync(plan);
                logger.LogInformation("Plano de {Days} dias criado.", plan.Days);
            }
        }
    }
}
