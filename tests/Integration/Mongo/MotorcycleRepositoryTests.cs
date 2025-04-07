using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MongoDB.Driver;
using MotoService.Domain.Entities;
using MotoService.Domain.Repositories;
using MotoService.Infrastructure.Persistence;
using Xunit;
using Mongo2Go;
using MotoService.Tests.Integration.Factories;

namespace MotoService.Tests.Integration.Mongo
{
    public class MotorcycleRepositoryTests : IAsyncLifetime
    {
        private MongoDbRunner _mongoRunner;
        private IMotorcycleRepository _repository;
        private MongoClient _client;
        private IClientSessionHandle _session;

        public async Task InitializeAsync()
        {
            (_mongoRunner, _client, _session, var mongoDbContext) = await MongoDbContextFactory.CreateAsync();

            var loggerMock = new Mock<ILogger<MotorcycleRepository>>();

            var sequenceGeneratorMock = new Mock<ISequenceGenerator>();
            sequenceGeneratorMock
                .Setup(s => s.GetNextSequenceValueAsync("moto"))
                .ReturnsAsync(1);

            _repository = new MotorcycleRepository(mongoDbContext, loggerMock.Object, sequenceGeneratorMock.Object);
        }

        public Task DisposeAsync()
        {
            _session?.Dispose();
            _mongoRunner?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllMotorcycles()
        {
            // Arrange
            var motorcycle = new Motorcycle(2025, "Model X", "ABC-1234");
            await _repository.CreateAsync(motorcycle);

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            result.Should().ContainSingle()
                .Which.Should().BeEquivalentTo(motorcycle, options => options.Excluding(m => m.Id));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMotorcycle_WhenExists()
        {
            // Arrange
            var motorcycle = new Motorcycle(2025, "Model X", "ABC-1234") { Identifier = "moto000001" };
            await _repository.CreateAsync(motorcycle);

            // Act
            var result = await _repository.GetByIdAsync("moto000001");

            // Assert
            result.Should().BeEquivalentTo(motorcycle, options => options.Excluding(m => m.Id));
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
        {
            // Act
            var result = await _repository.GetByIdAsync("moto999");

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task CreateAsync_ShouldInsertMotorcycle()
        {
            // Arrange
            var motorcycle = new Motorcycle(2025, "Model X", "ABC-1234");

            // Act
            var result = await _repository.CreateAsync(motorcycle);

            // Assert
            result.Should().Be("moto000001");
            var createdMotorcycle = await _repository.GetByIdAsync(result);
            createdMotorcycle.Should().BeEquivalentTo(motorcycle, options => options.Excluding(m => m.Id));
        }

        [Fact]
        public async Task UpdateAsync_ShouldModifyMotorcycle()
        {
            // Arrange
            var motorcycle = new Motorcycle(2025, "Model X", "ABC-1234") { Identifier = "moto000001" };
            await _repository.CreateAsync(motorcycle);
            
            // Act
            await _repository.UpdateAsync(motorcycle);
            var updatedMotorcycle = await _repository.GetByIdAsync("moto000001");

            // Assert
            updatedMotorcycle.Should().BeEquivalentTo(motorcycle, options => options.Excluding(m => m.Id));
        }

        [Fact]
        public async Task DeleteAsync_ShouldRemoveMotorcycle()
        {
            // Arrange
            var motorcycle = new Motorcycle(2025, "Model X", "ABC-1234") { Identifier = "moto000001" };
            await _repository.CreateAsync(motorcycle);

            // Act
            await _repository.DeleteAsync("moto000001");
            var result = await _repository.GetByIdAsync("moto000001");

            // Assert
            result.Should().BeNull();
        }
    }
}
