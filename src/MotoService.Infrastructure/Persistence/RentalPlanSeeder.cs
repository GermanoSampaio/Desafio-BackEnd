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

            var plansSection = configuration.GetSection("RentalPlans").Get<List<RentalPlan>>();
            if (plansSection == null || !plansSection.Any()) return;

            foreach (var plan in plansSection)
            {
                await rentalPlanRepository.CreateAsync(plan);
                logger.LogInformation("Plano de {Days} dias criado.", plan.Days);
            }
        }
    }
}
