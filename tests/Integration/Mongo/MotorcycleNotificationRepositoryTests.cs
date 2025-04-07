using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mongo2Go;
using MongoDB.Driver;
using Moq;
using MotoService.Domain.Entities;
using MotoService.Domain.Repositories;
using MotoService.Infrastructure.Persistence;
using MotoService.Tests.Integration.Factories;
using Xunit;

namespace MotoService.Tests.Integration.Mongo
{
    public class MotorcycleNotificationRepositoryTests : IAsyncLifetime
    {
        private MongoDbRunner _mongoRunner;
        private IMotorcycleNotificationRepository _repository;
        private MongoClient _client;
        private IClientSessionHandle _session;
        private IMongoCollection<MotorcycleRegisteredEvent> _collection;

        public async Task InitializeAsync()
        {
            (_mongoRunner, _client, _session, var mongoDbContext) = await MongoDbContextFactory.CreateAsync();

            var loggerMock = new Mock<ILogger<MotorcycleNotificationRepository>>();
            _repository = new MotorcycleNotificationRepository(mongoDbContext);
            _collection = mongoDbContext.MotorcycleNotifications;
        }

        public Task DisposeAsync()
        {
            _session?.Dispose();
            _mongoRunner?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task StoreNotificationAsync_ShouldPersistMotorcycleRegisteredEvent()
        {
            // Arrange
            var motorcycleEvent = new MotorcycleRegisteredEvent
            {
                Identifier = "event001",
                Year = 2025,
                Model = "Model X",
                LicensePlate = "ABC-1234",
                DateRegistered = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            // Act
            await _repository.StoreNotificationAsync(motorcycleEvent);
            var result = await _collection.Find(e => e.Identifier == "event001").FirstOrDefaultAsync();

            // Assert
            result.Should().NotBeNull();
            result.Identifier.Should().Be("event001");
            result.Year.Should().Be(2025);
            result.Model.Should().Be("Model X");
            result.LicensePlate.Should().Be("ABC-1234");
            result.DateRegistered.Should().Be(motorcycleEvent.DateRegistered);
        }
    }
}