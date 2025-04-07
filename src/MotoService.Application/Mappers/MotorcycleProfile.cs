using AutoMapper;
using MotoService.Application.DTOs;
using MotoService.Domain.Entities;

namespace MotoService.Application.Mappers
{
    public class MotorcycleProfile : Profile
    {
        public MotorcycleProfile()
        {
            CreateMap<CreateMotorcycleDTO, Motorcycle>()
                .ConstructUsing(dto => new Motorcycle(dto.Year, dto.Model, dto.LicensePlate));
            CreateMap<Motorcycle, MotorcycleDTO>();
        }
    }
}