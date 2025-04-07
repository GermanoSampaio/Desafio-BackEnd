using System;
using MotoService.Application.DTOs;

namespace MotoService.Application.Interfaces
{
    public interface IRentalService
    {
        Task<RentalDTO> GetRentalByIdAsync(string identifier);
        Task<RentalDTO> CreateRentalAsync(CreateRentalDTO rentalDto);
        Task UpdateRentalAsync(RentalDTO rentalDto);
    }
}
