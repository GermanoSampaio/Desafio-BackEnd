
using AutoMapper;
using Microsoft.Extensions.Logging;
using MotoService.Application.DTOs;
using MotoService.Application.Interfaces;
using MotoService.Application.Mappers;
using MotoService.Domain.Entities;
using MotoService.Domain.Exceptions;
using MotoService.Domain.Interfaces;
using MotoService.Infrastructure.Persistence;

namespace MotoService.Application.Services
{
    public class RentalService : IRentalService
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly ILogger<RentalService> _logger;
        private readonly IMapper _mapper;

        public RentalService(IRentalRepository rentalRepository,  ILogger<RentalService> logger,   IMapper mapper)
        {
            _rentalRepository = rentalRepository;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<RentalDTO> GetRentalByIdAsync(string identifier)
        {
            var rental = await _rentalRepository.GetRentalByIdAsync(identifier) ?? throw new RentalNotFoundException();

            return _mapper.Map<RentalDTO>(rental);
        }

        public async Task<RentalDTO> CreateRentalAsync(CreateRentalDTO rentalDto)
        {
            _logger.LogInformation("Criando locação para o entregador {DeliveryId}", rentalDto.DeliveryId);

            var rental = _mapper.Map<Rental>(rentalDto);
            var rentalCreated = await _rentalRepository.CreateRentalAsync(rental);

            _logger.LogInformation("Locação criada com ID {Identifier}", rentalCreated.Identifier);
            return _mapper.Map<RentalDTO>(rental);
        }

        public async Task UpdateRentalAsync(RentalDTO rentalDto)
        {
            var rental = await _rentalRepository.GetRentalByIdAsync(rentalDto.Identifier);
            if (rental is null)
                throw new RentalNotFoundException();

            rental.SetTerminalDate(rentalDto.TerminalDate);

            await _rentalRepository.UpdateRentalAsync(rental);
            _logger.LogInformation("Data de devolução atualizada para a locação {Identifier}", rentalDto.Identifier);
           
           
        }

    }
}
