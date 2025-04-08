using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MotoService.Domain.Exceptions;
using MotoService.Domain.Interfaces;

namespace MotoService.Domain.Services
{
    public class RentalDomainService : IRentalDomainService
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly ILogger<RentalDomainService> _logger;

        public RentalDomainService(IRentalRepository rentalRepository, ILogger<RentalDomainService> logger)
        {
            _rentalRepository = rentalRepository ?? throw new ArgumentNullException(nameof(rentalRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task ValidateRentalCreationAsync(string motorcycleId, string cnhType, DateTime startDate, DateTime endDate)
        {
            if (startDate.Date <= DateTime.UtcNow.Date)
                throw new RentalStartDateInvalidException();

            if (!cnhType.Contains("A"))
               throw new InvalidCnhTypeException(cnhType);

            await ValidateMotoAvailabilityAsync(motorcycleId, startDate, endDate);
        }


        private async Task ValidateMotoAvailabilityAsync(string motorcycleId, DateTime startDate, DateTime endDate)
        {
            var rentals = await _rentalRepository.GetRentalsByMotoIdAsync(motorcycleId);

            foreach (var rental in rentals)
            {
                bool isOverlapping =
                    (startDate < rental.TerminalDate && endDate > rental.StartDate);

                if (isOverlapping)
                {
                    _logger.LogWarning("Moto {motoId} já alugada entre {start} e {end}", motorcycleId, rental.StartDate, rental.TerminalDate);
                    throw new MotorcycleUnavailableException(motorcycleId);
                }
            }
        }
    }
}