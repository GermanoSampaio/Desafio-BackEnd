using AutoMapper;
using Microsoft.Extensions.Logging;
using MotoService.Application.DTOs;
using MotoService.Application.Interfaces;
using MotoService.Domain.Entities;
using MotoService.Domain.Exceptions;
using MotoService.Domain.Repositories;
using MotoService.Infrastructure.MessageBroker.Interfaces;

namespace MotoService.Application.Services
{
    public class MotorcycleService : IMotorcycleService
    {
        private readonly IMotorcycleRepository _motorcycleRepository;
        private readonly IRabbitMQPublisher _publisher;
        private readonly ILogger<MotorcycleService> _logger;
        private readonly IMapper _mapper;

        public MotorcycleService(IMotorcycleRepository motorcycleRepository, IRabbitMQPublisher publisher, ILogger<MotorcycleService> logger, IMapper mapper)
        {
            _motorcycleRepository = motorcycleRepository;
            _publisher = publisher;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IEnumerable<MotorcycleResponseDTO>> GetAllAsync()
        {
            var motorcycles = await _motorcycleRepository.GetAllAsync();
            return motorcycles.Select(m => _mapper.Map<MotorcycleResponseDTO>(m));
        }

        public async Task<MotorcycleResponseDTO?> GetByIdAsync(string identifier)
        {
            var motorcycle = await _motorcycleRepository.GetByIdAsync(identifier) ?? throw new MotorcycleNotFoundException();
            return _mapper.Map<MotorcycleResponseDTO>(motorcycle);
        }

        public async Task<bool> UpdateLicensePlateAsync(string id, string newPlate)
        {
            var motorcycle = await _motorcycleRepository.GetByIdAsync(id) ?? throw new MotorcycleNotFoundException();
            _logger.LogInformation("Alterando a Moto {Identifier}", id);
            motorcycle.SetLicensePlate(newPlate);
            await _motorcycleRepository.UpdateAsync(motorcycle);
            return true;
        }

        public async Task DeleteAsync(string identifier)
        {
            await _motorcycleRepository.DeleteAsync(identifier);
        }

        public async Task<string> CreateAsync(MotorcycleRequestDTO motorcycleDto)
        {
            var motorcycle = _mapper.Map<Motorcycle>(motorcycleDto);
            var created = await _motorcycleRepository.CreateAsync(motorcycle);

            if (created)
            {
                var registeredEvent = new MotorcycleRegisteredEvent
                {
                    Identifier = motorcycle.Identifier,
                    Year = motorcycle.Year,
                    Model = motorcycle.Model,
                    LicensePlate = motorcycle.LicensePlate
                };

                _logger.LogInformation("Moto cadastrada com sucesso. Publicando evento...");

                await _publisher.PublishMessageAsync(registeredEvent);
                _logger.LogInformation("Publicar notificação de moto cadastrada.");
            }
            else
            {
                _logger.LogError("Erro ao tentar cadastrar motocicleta.");
            }

            return motorcycle.Identifier;
        }
    }
}
