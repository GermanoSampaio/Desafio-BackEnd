using AutoMapper;
using MotoService.Application.DTOs;
using MotoService.Domain.Entities;

namespace MotoService.Application.Mappers
{
    public class DeliveryProfile : Profile
    {
        public DeliveryProfile()
        {
            CreateMap<DeliveryRequestDTO, Delivery>()
                .ConstructUsing(dto => new Delivery(
                    dto.Identifier,
                    dto.Name,
                    dto.Cnpj,
                    dto.BirthDate,
                    dto.CnhNumber,
                    dto.CnhType,
                    dto.CNHFileString
                ));
            CreateMap<Delivery, DeliveryResponseDTO>();
        }
    }
}
