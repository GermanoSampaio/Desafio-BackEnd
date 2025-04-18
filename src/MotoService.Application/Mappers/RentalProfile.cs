﻿using AutoMapper;
using MotoService.Application.DTOs;
using MotoService.Domain.Entities;

namespace MotoService.Application.Mappers
{
    public class RentalProfile : Profile
    {
        public RentalProfile()
        {
            CreateMap<RentalRequestDTO, Rental>()
                .ForMember(dest => dest.ExpectedTerminalDate, opt => opt.Ignore())
                .ForMember(dest => dest.RentalPlan, opt => opt.Ignore())
                .ForMember(dest => dest.DailyRate, opt => opt.Ignore());

            CreateMap<Rental, RentalResponseDTO>()
                .ForMember(dest => dest.RentalPlan, opt => opt.MapFrom(src => src.RentalPlan.Days));
        }
    }
}