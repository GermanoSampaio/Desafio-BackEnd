using AutoMapper;
using MotoService.Application.DTOs;
using MotoService.Domain.Entities;

namespace MotoService.Application.Mappers
{
    public class MotorcycleProfile : Profile
    {
        public MotorcycleProfile()
        {
            CreateMap<MotorcycleRequestDTO, Motorcycle>()
                .ConstructUsing(dto => new Motorcycle(dto.Identifier, dto.Year, dto.Model, dto.LicensePlate));
            CreateMap<Motorcycle, MotorcycleResponseDTO>();
        }
    }
}