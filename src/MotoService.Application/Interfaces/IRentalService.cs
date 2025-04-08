using System;
using MotoService.Application.DTOs;

namespace MotoService.Application.Interfaces
{
    public interface IRentalService
    {
        Task<RentalResponseDTO> GetRentalByIdAsync(string identifier);
        Task<RentalResponseDTO> CreateRentalAsync(RentalRequestDTO rentalDto);
        Task UpdateRentalAsync(RentalResponseDTO rentalDto, DateTime newTerminalDate);
    }
}
