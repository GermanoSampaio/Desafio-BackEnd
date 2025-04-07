using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MotoService.Application.DTOs;
using MotoService.Domain.Entities;

namespace MotoService.Application.Interfaces
{
    public interface IMotorcycleService
    {
        Task<IEnumerable<MotorcycleDTO>> GetAllAsync();
        Task<MotorcycleDTO?> GetByIdAsync(string identifier);
        Task<string> CreateAsync(CreateMotorcycleDTO motorcycleDto);
        Task<bool> UpdateLicensePlateAsync(string id, string newPlate);
        Task DeleteAsync(string identifier);
    }
}
