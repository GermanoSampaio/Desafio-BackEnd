using System.Xml.Linq;
using AutoMapper;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using MotoService.Application.DTOs;
using MotoService.Application.Mappers;
using MotoService.Application.Services;
using MotoService.Domain.Entities;
using MotoService.Domain.Enums;
using MotoService.Domain.Exceptions;
using MotoService.Domain.Interfaces;
using MotoService.Domain.Repositories;
using Xunit;

namespace tests.Unit.ServiceTests
{
    public class RentalServiceTests
    {
        private readonly Mock<IRentalRepository> _mockRentalRepository;
        private readonly Mock<IDeliveryRepository> _mockDeliveryRepository;
        private readonly Mock<IMotorcycleRepository> _mockMotorcycleRepository;
        private readonly Mock<IRentalDomainService> _mockRentalDomainService;
        private readonly Mock<ILogger<RentalService>> _mockLogger;
        private readonly IMapper _mapper;
        private readonly RentalService _rentalService;

        public RentalServiceTests()
        {
            _mockRentalRepository = new Mock<IRentalRepository>();
            _mockDeliveryRepository = new Mock<IDeliveryRepository>();
            _mockMotorcycleRepository = new Mock<IMotorcycleRepository>();
            _mockRentalDomainService = new Mock<IRentalDomainService>();
            _mockLogger = new Mock<ILogger<RentalService>>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<RentalProfile>());
            _mapper = config.CreateMapper();
            _rentalService = new RentalService(_mockRentalRepository.Object, _mockMotorcycleRepository.Object, _mockDeliveryRepository.Object, _mockRentalDomainService.Object,_mockLogger.Object, _mapper);
        }

        [Fact]
        public async Task GetRentalByIdAsync_ShouldReturnRental_WhenExists()
        {
            // Arrange
            var rental = new Rental("entregador2637", "delivery001", "moto001", DateTime.UtcNow, DateTime.UtcNow.AddDays(2))
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
            var createDto = new RentalRequestDTO
            {
                DeliveryId = "delivery001",
                MotorcycleId = "moto001",
                StartDate = DateTime.UtcNow,
                TerminalDate = DateTime.UtcNow.AddDays(2)
            };
           

            var rental = new Rental("entregador2637", "delivery001", "moto001", createDto.StartDate, createDto.TerminalDate)
            {
                Identifier = "fed39c68d391480597bbab886669f0d4" 
            };
            _mockMotorcycleRepository.Setup(x => x.GetByIdAsync("moto001"))
             .ReturnsAsync(new Motorcycle("moto001",2024, "Honda", "CGA-1600"));

            var base64Image = Convert.ToBase64String(new byte[] { 0x89, 0x50, 0x4E, 0x47 });

            _mockDeliveryRepository.Setup(x => x.GetByIdentifierAsync("delivery001"))
                .ReturnsAsync(new Delivery( "delivery001",
                 "João da Silva",
                "12345678000199",
                new DateOnly(2000, 5, 4),
                 "12345678900",
                "A",
                base64Image));

            _mockRentalRepository.Setup(x => x.CreateRentalAsync(It.IsAny<Rental>()))
                .ReturnsAsync(rental);

            // Act
            var result = await _rentalService.CreateRentalAsync(createDto);

            // Assert
            result.Should().NotBeNull();
            result.Identifier.Should().NotBeNullOrEmpty();
        }


        [Fact]
        public async Task UpdateRentalAsync_ShouldThrowRentalNotFoundException_WhenNotExists()
        {
            // Arrange
            var rentalDto = new RentalResponseDTO
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
            Func<Task> act = async () => await _rentalService.UpdateRentalAsync(rentalDto, rentalDto.TerminalDate);

            // Assert
            await act.Should().ThrowAsync<RentalNotFoundException>();
        }
    }
}

