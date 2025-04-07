using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MotoService.Domain.Entities;
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
                long next = await _sequenceGenerator.GetNextSequenceValueAsync("rentals");
                rental.Identifier = $"rental{next:D6}";

                var rentalDays = (rental.TerminalDate - rental.StartDate).Days;
                var plan = await _rentalPlanRepository.GetByDaysAsync(rentalDays);

                if (plan is null)
                {
                    _logger.LogWarning("Plano de locação não encontrado para {rentalDays} dias.", rentalDays);
                    return null;
                }

                double totalPrice = RentalPriceCalculator.CalculateTotal(rental, plan);

                rental.SetRentalPlan(plan);

                rental.SetRentalPlan(plan);
                rental.SetExpectedTerminalDate(rental.StartDate.AddDays(plan.Days));
                rental.SetDailyRate(totalPrice);

                await _rentalCollection.InsertOneAsync(rental);

                _logger.LogInformation("Locação criada com sucesso: {Identifier}", rental.Identifier);

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

        public async Task UpdateRentalAsync(Rental rental)
        {
            var filter = Builders<Rental>.Filter.Eq(r => r.Identifier, rental.Identifier);
            var result = await _rentalCollection.ReplaceOneAsync(filter, rental);

            if (result.ModifiedCount == 0)
                _logger.LogWarning("Nenhuma locação atualizada para o ID {Identifier}.", rental.Identifier);
            else
                _logger.LogInformation("Locação atualizada com sucesso: {Identifier}", rental.Identifier);
        }

    }
}