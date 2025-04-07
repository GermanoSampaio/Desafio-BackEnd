using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MotoService.Domain.Entities;

namespace MotoService.Domain.Interfaces
{
    public interface IRentalRepository
    {
        Task<Rental?> GetRentalByIdAsync(string identifier);
        Task<Rental> CreateRentalAsync(Rental rentalDto);
        Task UpdateRentalAsync(Rental rentalDto);
    }
}
