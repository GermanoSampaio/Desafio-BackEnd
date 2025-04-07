using System;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MotoService.Application.DTOs;
using MotoService.Application.Mappers;
using MotoService.Application.Services;
using MotoService.Domain.Entities;
using MotoService.Domain.Exceptions;
using MotoService.Domain.Interfaces;
using Xunit;

namespace tests.Unit.ServiceTests
{
    public class RentalServiceTests
    {
        private readonly Mock<IRentalRepository> _mockRentalRepository;
        private readonly Mock<ILogger<RentalService>> _mockLogger;
        private readonly IMapper _mapper;
        private readonly RentalService _rentalService;

        public RentalServiceTests()
        {
            _mockRentalRepository = new Mock<IRentalRepository>();
            _mockLogger = new Mock<ILogger<RentalService>>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<RentalProfile>());
            _mapper = config.CreateMapper();
            _rentalService = new RentalService(_mockRentalRepository.Object, _mockLogger.Object, _mapper);
        }

        [Fact]
        public async Task GetRentalByIdAsync_ShouldReturnRental_WhenExists()
        {
            // Arrange
            var rental = new Rental("delivery001", "moto001", DateTime.UtcNow, DateTime.UtcNow.AddDays(2))
            {
                Identifier = "rental001"
            };

            _mockRentalRepository.Setup(x => x.GetRentalByIdAsync("rental001"))
                .ReturnsAsync(rental);

            // Act
            var result = await _rentalService.GetRentalByIdAsync("rental001");

            // Assert
            result.Should().NotBeNull();
            result.Identifier.Should().Be("rental001");
        }

        [Fact]
        public async Task GetRentalByIdAsync_ShouldThrowRentalNotFoundException_WhenNotExists()
        {
            // Arrange
            _mockRentalRepository.Setup(x => x.GetRentalByIdAsync("rental999"))
                .ReturnsAsync((Rental)null);

            // Act
            Func<Task> act = async () => await _rentalService.GetRentalByIdAsync("rental999");

            // Assert
            await act.Should().ThrowAsync<RentalNotFoundException>();
        }

        [Fact]
        public async Task CreateRentalAsync_ShouldReturnRental_WhenValid()
        {
            // Arrange
            var createDto = new CreateRentalDTO
            {
                DeliveryId = "delivery001",
                MotorcycleId = "moto001",
                StartDate = DateTime.UtcNow,
                TerminalDate = DateTime.UtcNow.AddDays(2)
            };

            var rental = new Rental("delivery001", "moto001", createDto.StartDate, createDto.TerminalDate)
            {
                Identifier = "fed39c68d391480597bbab886669f0d4" // Exemplo de identificador gerado automaticamente
            };

            _mockRentalRepository.Setup(x => x.CreateRentalAsync(It.IsAny<Rental>()))
                .ReturnsAsync(rental);

            // Act
            var result = await _rentalService.CreateRentalAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Identifier.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task UpdateRentalAsync_ShouldUpdateTerminalDate_WhenValid()
        {
            // Arrange
            var rental = new Rental("delivery001", "moto001", DateTime.UtcNow, DateTime.UtcNow.AddDays(2))
            {
                Identifier = "rental001"
            };

            var rentalDto = new RentalDTO
            {
                Identifier = "rental001",
                DeliveryId = "delivery001",
                MotorcycleId = "moto001",
                StartDate = DateTime.UtcNow,
                TerminalDate = DateTime.UtcNow.AddDays(4)
            };

            _mockRentalRepository.Setup(x => x.GetRentalByIdAsync("rental001"))
                .ReturnsAsync(rental);

            // Act
            await _rentalService.UpdateRentalAsync(rentalDto);

            // Assert
            _mockRentalRepository.Verify(x => x.UpdateRentalAsync(It.Is<Rental>(r => r.TerminalDate == rentalDto.TerminalDate)), Times.Once);
        }

        [Fact]
        public async Task UpdateRentalAsync_ShouldThrowRentalNotFoundException_WhenNotExists()
        {
            // Arrange
            var rentalDto = new RentalDTO
            {
                Identifier = "rental999",
                DeliveryId = "delivery001",
                MotorcycleId = "moto001",
                StartDate = DateTime.UtcNow,
                TerminalDate = DateTime.UtcNow.AddDays(4)
            };

            _mockRentalRepository.Setup(x => x.GetRentalByIdAsync("rental999"))
                .ReturnsAsync((Rental)null);

            // Act
            Func<Task> act = async () => await _rentalService.UpdateRentalAsync(rentalDto);

            // Assert
            await act.Should().ThrowAsync<RentalNotFoundException>();
        }
    }
}

