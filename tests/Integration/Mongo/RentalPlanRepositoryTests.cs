using System;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mongo2Go;
using MongoDB.Bson;
using MongoDB.Driver;
using Moq;
using MotoService.Domain.Entities;
using MotoService.Domain.Interfaces;
using MotoService.Infrastructure.Persistence;
using MotoService.Infrastructure.Persistence.Contexts;
using MotoService.Infrastructure.Persistence.Settings;
using MotoService.Tests.Integration.Factories;
using Xunit;

namespace MotoService.Tests.Integration.Mongo
{
    public class RentalPlanRepositoryTests : IAsyncLifetime
    {
        private MongoDbRunner _mongoRunner;
        private IRentalPlanRepository _repository;
        private MongoClient _client;
        private IClientSessionHandle _session;

        public async Task InitializeAsync()
        {
            (_mongoRunner, _client, _session, var mongoDbContext) = await MongoDbContextFactory.CreateAsync();

            var loggerMock = new Mock<ILogger<RentalPlanRepository>>();
            _repository = new RentalPlanRepository(mongoDbContext);
        }

        public Task DisposeAsync()
        {
            _session?.Dispose();
            _mongoRunner?.Dispose();
            return Task.CompletedTask;
        }

        [Fact]
        public async Task GetByDaysAsync_ShouldReturnNull_WhenNotExists()
        {
            // Act
            var result = await _repository.GetByDaysAsync(10);

            // Assert
            result.Should().BeNull();
        }
    }
}
