using System.Data;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MotoService.Domain.Entities;
using MotoService.Domain.Exceptions;
using MotoService.Domain.Interfaces;
using MotoService.Domain.Repositories;
using MotoService.Domain.Services;
using MotoService.Infrastructure.Persistence.Contexts;

namespace MotoService.Infrastructure.Persistence
{
    public class RentalRepository : IRentalRepository
    {
        private readonly IMongoCollection<Rental> _rentalCollection;
        private readonly ISequenceGenerator _sequenceGenerator;
        private readonly IRentalPlanRepository _rentalPlanRepository;
        private readonly ILogger<RentalRepository> _logger;

        public RentalRepository(
            MongoDbContext context,
            ISequenceGenerator sequenceGenerator,
            IRentalPlanRepository rentalPlanRepository,
            ILogger<RentalRepository> logger)
        {
            _rentalCollection = context.Rental ?? throw new ArgumentNullException(nameof(context.Rental));
            _sequenceGenerator = sequenceGenerator ?? throw new ArgumentNullException(nameof(sequenceGenerator));
            _rentalPlanRepository = rentalPlanRepository ?? throw new ArgumentNullException(nameof(rentalPlanRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public async Task<Rental?> GetRentalByIdAsync(string id)
        {
            return await _rentalCollection
                .Find(r => r.Identifier == id)
                .FirstOrDefaultAsync();
        }

        public async Task<Rental> CreateRentalAsync(Rental rental)
        {
            try
            {
                var next = await _sequenceGenerator.GetNextSequenceValueAsync("rentals");
                rental.SetIdentifier($"locacao{next}");

                var configuredRental = await ConfigureRentalPlanAsync(rental);

                await _rentalCollection.InsertOneAsync(configuredRental);

                _logger.LogInformation("Locação criada com sucesso: {Identifier}", configuredRental.Identifier);

                return rental;
            }
            catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
            {
                _logger.LogError(ex, "Erro de chave duplicada ao inserir a locação: {Message}", ex.Message);
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro inesperado ao criar locação: {Message}", ex.Message);
                throw; 
            }
        }

        public async Task UpdateRentalAsync(Rental rental, DateTime newTerminalDate)
        {
            var totalRental =  RentalPriceCalculator.CalculateTotal(rental, newTerminalDate);
            rental.SetTerminalDate(newTerminalDate);
            rental.SetDailyRate(totalRental);

            var filter = Builders<Rental>.Filter.Eq(r => r.Identifier, rental.Identifier);
            var result = await _rentalCollection.ReplaceOneAsync(filter, rental);

            if (result.ModifiedCount == 0)
                _logger.LogWarning("Nenhuma locação atualizada para o ID {Identifier}.", rental.Identifier);
            else
                _logger.LogInformation("Locação atualizada com sucesso: {Identifier}", rental.Identifier);
        }

        private async Task<Rental?> ConfigureRentalPlanAsync(Rental rental)
        {
            var plan = await _rentalPlanRepository.GetByDaysAsync(rental.RentalPlan.Days);

            if (plan is null)
            {
                _logger.LogWarning("Plano de locação não encontrado para {rentalDays} dias.", rental.RentalPlan.Days);
                throw new RentalPlanNotFoundException(); 
            }

            var totalRental = RentalPriceCalculator.CalculateTotal(rental, rental.TerminalDate);

            rental.SetRentalPlan(plan);
            rental.SetDailyRate(totalRental);
            rental.SetExpectedTerminalDate(rental.StartDate.AddDays(plan.Days));

            return rental;
        }

        public async Task<List<Rental>> GetRentalsByMotoIdAsync(string motorcycleId)
        {
            return await _rentalCollection
                .Find(r => r.MotorcycleId == motorcycleId)
                .ToListAsync();
        }


    }
}