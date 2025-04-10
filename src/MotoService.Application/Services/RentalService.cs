
using AutoMapper;
using Microsoft.Extensions.Logging;
using MotoService.Application.DTOs;
using MotoService.Application.Interfaces;
using MotoService.Domain.Entities;
using MotoService.Domain.Exceptions;
using MotoService.Domain.Interfaces;
using MotoService.Domain.Repositories;
using MotoService.Domain.Services;
using MotoService.Infrastructure.Persistence;

namespace MotoService.Application.Services
{
    public class RentalService : IRentalService
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly IMotorcycleRepository _motorcycleRepository;
        private readonly IDeliveryRepository _deliveryRepository;
        private readonly IRentalDomainService _rentalDomainService;
        private readonly ILogger<RentalService> _logger;
        private readonly IMapper _mapper;

        public RentalService(IRentalRepository rentalRepository, IMotorcycleRepository motorcycleRepository, IDeliveryRepository deliveryRepository, IRentalDomainService rentalDomainService, ILogger<RentalService> logger,   IMapper mapper)
        {
            _rentalRepository = rentalRepository;
            _motorcycleRepository = motorcycleRepository;
            _deliveryRepository = deliveryRepository;
            _rentalDomainService = rentalDomainService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<RentalResponseDTO> GetRentalByIdAsync(string identifier)
        {
            var rental = await _rentalRepository.GetRentalByIdAsync(identifier) ?? throw new RentalNotFoundException();

            return _mapper.Map<RentalResponseDTO>(rental);
        }

        public async Task<RentalResponseDTO> CreateRentalAsync(RentalRequestDTO rentalDto)
        {
            _logger.LogInformation("Criando locação para o entregador {DeliveryId}", rentalDto.DeliveryId);

            var motorcycle = await _motorcycleRepository.GetByIdAsync(rentalDto.MotorcycleId) ?? throw new MotorcycleNotFoundException();

            var delivery = await _deliveryRepository.GetByIdentifierAsync(rentalDto.DeliveryId) ?? throw new DeliveryNotFoundException();

            await _rentalDomainService.ValidateRentalCreationAsync(rentalDto.MotorcycleId, delivery.CnhType, rentalDto.StartDate, rentalDto.TerminalDate);

            var rental = _mapper.Map<Rental>(rentalDto);
            
            var rentalCreated = await _rentalRepository.CreateRentalAsync(rental);

            _logger.LogInformation("Locação criada com ID {Identifier}", rentalCreated.Identifier);
            return _mapper.Map<RentalResponseDTO>(rental);
        }

        public async Task UpdateRentalAsync(RentalResponseDTO rentalDto, DateTime newTerminalDate)
        {
            var rental = await _rentalRepository.GetRentalByIdAsync(rentalDto.Identifier);
            if (rental is null)
                throw new RentalNotFoundException();

            await _rentalRepository.UpdateRentalAsync(rental, newTerminalDate);
            _logger.LogInformation("Data de devolução atualizada para a locação {Identifier}", rentalDto.Identifier);
           
           
        }

    }
}
