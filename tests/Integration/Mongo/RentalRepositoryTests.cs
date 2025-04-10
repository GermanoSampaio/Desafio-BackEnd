using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using MotoService.Domain.Entities;
using MotoService.Domain.Interfaces;
using MotoService.Domain.Repositories;
using MotoService.Infrastructure.Persistence;
using MotoService.Tests.Integration.Factories;
using Xunit;

namespace tests.Integration.Mongo
{
    public class RentalRepositoryTests : IAsyncLifetime
{
    private MongoDbRunner _mongoRunner;
    private IRentalRepository _repository;
    private MongoClient _client;
    private IClientSessionHandle _session;

    public async Task InitializeAsync()
    {
        (_mongoRunner, _client, _session, var mongoDbContext) = await MongoDbContextFactory.CreateAsync();

            var loggerMock = new Mock<ILogger<RentalRepository>>();
             var sequenceGeneratorMock = new Mock<ISequenceGenerator>();
            sequenceGeneratorMock.Setup(s => s.GetNextSequenceValueAsync("rentals")).ReturnsAsync("locacao123");

            var rentalPlanRepositoryMock = new Mock<IRentalPlanRepository>();
            rentalPlanRepositoryMock.Setup(r => r.GetByDaysAsync(It.IsAny<int>())).ReturnsAsync(new RentalPlan(3, 75) { Id = ObjectId.GenerateNewId().ToString() });

            _repository = new RentalRepository(mongoDbContext, sequenceGeneratorMock.Object, rentalPlanRepositoryMock.Object, loggerMock.Object);
        }

    public Task DisposeAsync()
    {
        _session?.Dispose();
        _mongoRunner?.Dispose();
        return Task.CompletedTask;
    }

    [Fact]
        public async Task InsertAsync_ShouldPersistRental()
        {
            // Arrange
            var rental = new Rental("entregador2637", "deliv001", "moto001", DateTime.Today, DateTime.Today.AddDays(7));
            rental.SetExpectedTerminalDate(DateTime.Today.AddDays(3));
            rental.SetDailyRate(75);
            rental.SetRentalPlan(new RentalPlan(3, 75));

            // Act
            await _repository.CreateRentalAsync(rental);
            var result = await _repository.GetRentalByIdAsync(rental.Identifier);

            // Assert
            result.Should().NotBeNull();
            result.DeliveryId.Should().Be("deliv001");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnRental_WhenRentalExists()
        {
            // Arrange
            var rental = new Rental("entregador2637", "deliv002", "moto002", DateTime.Today, DateTime.Today.AddDays(5));
            rental.SetExpectedTerminalDate(DateTime.Today.AddDays(5));
            rental.SetDailyRate(80);
            rental.SetRentalPlan(new RentalPlan(5, 80));

            await _repository.CreateRentalAsync(rental);

            // Act
            var result = await _repository.GetRentalByIdAsync(rental.Identifier);

            // Assert
            result.Should().NotBeNull();
            result.Identifier.Should().Be(rental.Identifier);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenRentalDoesNotExist()
        {
            // Act
            var result = await _repository.GetRentalByIdAsync("000000000000000000000000");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyRental()
        {
            // Arrange
            var rental = new Rental("entregador2637", "deliv003", "moto003", DateTime.Today, DateTime.Today.AddDays(7));
            rental.SetExpectedTerminalDate(DateTime.Today.AddDays(7));
            rental.SetDailyRate(90);
            rental.SetRentalPlan(new RentalPlan(7, 90) { Id = ObjectId.GenerateNewId().ToString() });

            await _repository.CreateRentalAsync(rental);

            rental.SetDailyRate(100);
            var newTerminalDate = DateTime.Today.AddDays(10); 

            // Act
            await _repository.UpdateRentalAsync(rental, newTerminalDate); 
            var result = await _repository.GetRentalByIdAsync(rental.Identifier);

            // Assert
            result.Should().NotBeNull();
            result.DailyRate.Should().Be(650);
        }
    }
}
