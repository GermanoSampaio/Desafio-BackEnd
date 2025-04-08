using FluentAssertions;
using Microsoft.Extensions.Logging;
using Mongo2Go;
using MongoDB.Driver;
using Moq;
using MotoService.Domain.Entities;
using MotoService.Domain.Enums;
using MotoService.Domain.Interfaces;
using MotoService.Domain.Repositories;
using MotoService.Infrastructure.Persistence;
using MotoService.Tests.Integration.Factories;

namespace tests.Integration.Mongo
{
    public class DeliveryRepositoryTests : IAsyncLifetime
    {
        private MongoDbRunner _mongoRunner;
        private IDeliveryRepository _repository;
        private MongoClient _client;
        private IClientSessionHandle _session;

        public async Task InitializeAsync()
        {
            (_mongoRunner, _client, _session, var mongoDbContext) = await MongoDbContextFactory.CreateAsync();

            var loggerMock = new Mock<ILogger<DeliveryRepository>>();

            _repository = new DeliveryRepository(loggerMock.Object, mongoDbContext);
            await _repository.EnsureIndexesAsync(); 
        }

        public Task DisposeAsync()
        {
            _session?.Dispose();
            _mongoRunner?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task CreateAsync_ShouldInsertDelivery()
        {
            // Arrange
            var delivery = new Delivery("entregador123", "João", "91.393.713/0001-18", new DateOnly(1990, 5, 4), "73594488006", "A", "base64String");

            // Act 
            await _repository.CreateAsync(delivery);

            // Assert
            var result = await _repository.GetByIdentifierAsync(delivery.Identifier);
            result.Should().NotBeNull();
            result!.Name.Should().Be("João");
            result.CnhNumber.Should().Be("73594488006");
        }

        [Fact]
        public async Task ExistsByCNPJAsync_ShouldReturnTrue_WhenCNPJExists()
        {
            var delivery = new Delivery("entregador555", "Maria", "11.945.466/0001-86", new DateOnly(1990, 5, 4), "84825309187", "A", "base64String");

            // Act
            await _repository.CreateAsync(delivery);

            // Act
            var exists = await _repository.ExistsByCNPJAsync(delivery.Cnpj);

            // Assert
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsByCNHAsync_ShouldReturnTrue_WhenCNHExists()
        {
            var delivery = new Delivery("entregador598", "Carlos", "25.867.328/0001-30", new DateOnly(1990, 5, 4), "53262624680", "A", "base64String");

            // Act
            await _repository.CreateAsync(delivery);

            // Act
            var exists = await _repository.ExistsByCNHAsync(delivery.CnhNumber);

            // Assert
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateCNHImageUrlAsync_ShouldUpdateImageUrl()
        {
            var delivery = new Delivery("entregador599","Ana", "82.236.017/0001-07", new DateOnly(1990, 5, 4), "59440509314", "A", "base64String");
            await _repository.CreateAsync(delivery);

            delivery.SetCnhImageUrl("https://bucket/cnh/image.jpg");
            await _repository.UpdateCNHImageUrlAsync(delivery);

            var updated = await _repository.GetByIdentifierAsync(delivery.Identifier);

            updated!.CnhImageURL.Should().Be("https://bucket/cnh/image.jpg");
        }
    }
}