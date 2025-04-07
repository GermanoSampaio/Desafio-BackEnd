using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MotoService.Domain.Entities;

namespace MotoService.Domain.Interfaces
{
    public interface IRentalPlanRepository
    {
        Task<RentalPlan?> GetByDaysAsync(int days);
    }
}
