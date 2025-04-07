using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MotoService.Application.DTOs;
using MotoService.Application.Mappers;
using MotoService.Application.Services;
using MotoService.Domain.Entities;
using MotoService.Domain.Exceptions;
using MotoService.Domain.Repositories;
using MotoService.Infrastructure.MessageBroker.Interfaces;
using Xunit;

namespace MotoService.Tests.Unit.ServiceTests
{
    public class MotorcycleServiceTests
    {
        private readonly Mock<IMotorcycleRepository> _mockMotorcycleRepository;
        private readonly Mock<IRabbitMQPublisher> _mockPublisher;
        private readonly Mock<ILogger<MotorcycleService>> _mockLogger;
        private readonly IMapper _mapper;
        private readonly MotorcycleService _motorcycleService;

        public MotorcycleServiceTests()
        {
            _mockMotorcycleRepository = new Mock<IMotorcycleRepository>();
            _mockPublisher = new Mock<IRabbitMQPublisher>();
            _mockLogger = new Mock<ILogger<MotorcycleService>>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<MotorcycleProfile>());
            _mapper = config.CreateMapper();
            _motorcycleService = new MotorcycleService(_mockMotorcycleRepository.Object, _mockPublisher.Object, _mockLogger.Object, _mapper);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllMotorcycles()
        {
            // Arrange
            var motorcycles = new List<Motorcycle>
            {
                new Motorcycle(2020, "Model1", "ABC-1234") { Identifier = "moto001" },
                new Motorcycle(2021, "Model2", "DEF-5678") { Identifier = "moto002" }
            };

            _mockMotorcycleRepository.Setup(x => x.GetAllAsync()).ReturnsAsync(motorcycles);

            // Act
            var result = await _motorcycleService.GetAllAsync();

            // Assert
            result.Should().HaveCount(2);
            result.First().Identifier.Should().Be("moto001");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnMotorcycle_WhenExists()
        {
            // Arrange
            var motorcycle = new Motorcycle(2020, "Model1", "ABC-1234") { Identifier = "moto001" };

            _mockMotorcycleRepository.Setup(x => x.GetByIdAsync("moto001")).ReturnsAsync(motorcycle);

            // Act
            var result = await _motorcycleService.GetByIdAsync("moto001");

            // Assert
            result.Should().NotBeNull();
            result.Identifier.Should().Be("moto001");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrowMotorcycleNotFoundException_WhenNotExists()
        {
            // Arrange
            _mockMotorcycleRepository.Setup(x => x.GetByIdAsync("moto999")).ReturnsAsync((Motorcycle)null);

            // Act
            Func<Task> act = async () => await _motorcycleService.GetByIdAsync("moto999");

            // Assert
            await act.Should().ThrowAsync<MotorcycleNotFoundException>();
        }

        [Fact]
        public async Task UpdateLicensePlateAsync_ShouldUpdateLicensePlate_WhenValid()
        {
            // Arrange
            var motorcycle = new Motorcycle(2020, "Model1", "ABC-1234") { Identifier = "moto001" };

            _mockMotorcycleRepository.Setup(x => x.GetByIdAsync("moto001")).ReturnsAsync(motorcycle);

            // Act
            var result = await _motorcycleService.UpdateLicensePlateAsync("moto001", "XYZ-9876");

            // Assert
            result.Should().BeTrue();
            _mockMotorcycleRepository.Verify(x => x.UpdateAsync(It.Is<Motorcycle>(m => m.LicensePlate == "XYZ-9876")), Times.Once);
        }

        [Fact]
        public async Task UpdateLicensePlateAsync_ShouldThrowMotorcycleNotFoundException_WhenNotExists()
        {
            // Arrange
            _mockMotorcycleRepository.Setup(x => x.GetByIdAsync("moto999")).ReturnsAsync((Motorcycle)null);

            // Act
            Func<Task> act = async () => await _motorcycleService.UpdateLicensePlateAsync("moto999", "XYZ-9876");

            // Assert
            await act.Should().ThrowAsync<MotorcycleNotFoundException>();
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteMotorcycle_WhenExists()
        {
            // Act
            await _motorcycleService.DeleteAsync("moto001");

            // Assert
            _mockMotorcycleRepository.Verify(x => x.DeleteAsync("moto001"), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ShouldReturnIdentifier_WhenValid()
        {
            // Arrange
            var createDto = new CreateMotorcycleDTO
            {
                Model = "Model1",
                Year = 2020,
                LicensePlate = "ABC-1234"
            };

            var motorcycle = new Motorcycle(2020, "Model1", "ABC-1234")
            {
                Identifier = "moto001"
            };

            _mockMotorcycleRepository.Setup(x => x.CreateAsync(It.IsAny<Motorcycle>())).ReturnsAsync("moto001");

            // Act
            var result = await _motorcycleService.CreateAsync(createDto);

            // Assert
            result.Should().Be("moto001");
            _mockPublisher.Verify(x => x.PublishMessageAsync(It.IsAny<MotorcycleRegisteredEvent>()), Times.Once);
        }
    }
}
