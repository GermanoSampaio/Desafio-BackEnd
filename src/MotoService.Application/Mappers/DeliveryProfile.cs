using AutoMapper;
using MotoService.Application.DTOs;
using MotoService.Domain.Entities;

namespace MotoService.Application.Mappers
{
    public class DeliveryProfile : Profile
    {
        public DeliveryProfile()
        {
            CreateMap<CreateDeliveryDTO, Delivery>()
                .ConstructUsing(dto => new Delivery(
                    dto.Name,
                    dto.Cnpj,
                    dto.BirthDate,
                    dto.CnhNumber,
                    dto.CnhType
                ));
            CreateMap<Delivery, DeliveryDTO>();
        }
    }
}
