using MotoService.Application.DTOs;
using MotoService.Domain.Entities;

namespace MotoService.Application.Mappers
{
    public static class DeliveryMapper
    {
        public static Delivery ToEntity(this CreateDeliveryDTO dto)
        {
            var delivery = new Delivery(dto.Name, dto.Cnpj, dto.BirthDate, dto.CnhNumber, dto.CnhType);

            if (!string.IsNullOrWhiteSpace(dto.Id))
                delivery.Id = dto.Id;

            return delivery;
        }

        public static DeliveryDTO ToDTO(this Delivery entity)
        {
            return new DeliveryDTO
            {
                Id = entity.Id,
                Identifier = entity.Identifier,
                Name = entity.Name,
                Cnpj = entity.Cnpj,
                BirthDate = entity.BirthDate,
                CnhNumber = entity.CnhNumber,
                CnhType = entity.CnhType,
                CnhImageURL = entity.CnhImageURL
            };
        }
    }
}