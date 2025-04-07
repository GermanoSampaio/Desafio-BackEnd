using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mongo2Go;
using MongoDB.Driver;
using Moq;
using MotoService.Domain.Entities;
using MotoService.Domain.Enums;
using MotoService.Domain.Interfaces;
using MotoService.Domain.Repositories;
using MotoService.Infrastructure.Persistence;
using MotoService.Infrastructure.Persistence.Contexts;
using MotoService.Infrastructure.Persistence.Settings;
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

            var sequenceGeneratorMock = new Mock<ISequenceGenerator>();
            sequenceGeneratorMock
                .Setup(s => s.GetNextSequenceValueAsync("delivery"))
                .ReturnsAsync(1);

            _repository = new DeliveryRepository(loggerMock.Object, mongoDbContext, sequenceGeneratorMock.Object);
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
            var delivery = new Delivery("João", "91.393.713/0001-18", new DateTime(1990, 5, 4), "73594488006", CnhType.A);

            // Act - Chama o método de criação real
            await _repository.CreateAsync(delivery);

            // Assert - Verifica se o delivery foi inserido no banco
            var result = await _repository.GetByIdentifierAsync(delivery.Identifier);
            result.Should().NotBeNull();
            result!.Name.Should().Be("João");
            result.CnhNumber.Should().Be("73594488006");
        }

        [Fact]
        public async Task ExistsByCNPJAsync_ShouldReturnTrue_WhenCNPJExists()
        {
            var delivery = new Delivery("Maria", "11.945.466/0001-86", new DateTime(1990, 5, 4), "84825309187", CnhType.A);

            // Act - Cria o delivery no banco
            await _repository.CreateAsync(delivery);

            // Act - Verifica se o CNPJ existe
            var exists = await _repository.ExistsByCNPJAsync(delivery.Cnpj);

            // Assert
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task ExistsByCNHAsync_ShouldReturnTrue_WhenCNHExists()
        {
            var delivery = new Delivery("Carlos", "25.867.328/0001-30", new DateTime(1990, 5, 4), "53262624680", CnhType.A);

            // Act - Cria o delivery no banco
            await _repository.CreateAsync(delivery);

            // Act - Verifica se a CNH existe
            var exists = await _repository.ExistsByCNHAsync(delivery.CnhNumber);

            // Assert
            exists.Should().BeTrue();
        }

        [Fact]
        public async Task UpdateCNHImageUrlAsync_ShouldUpdateImageUrl()
        {
            var delivery = new Delivery("Ana", "82.236.017/0001-07", new DateTime(1990, 5, 4), "59440509314", CnhType.A);
            await _repository.CreateAsync(delivery);

            delivery.SetCnhImageUrl("https://bucket/cnh/image.jpg");
            await _repository.UpdateCNHImageUrlAsync(delivery);

            var updated = await _repository.GetByIdentifierAsync(delivery.Identifier);

            updated!.CnhImageURL.Should().Be("https://bucket/cnh/image.jpg");
        }
    }
}