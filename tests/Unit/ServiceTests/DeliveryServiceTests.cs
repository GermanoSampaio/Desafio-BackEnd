using AutoMapper;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Moq;
using MotoService.Application.DTOs;
using MotoService.Application.Mappers;
using MotoService.Application.Services;
using MotoService.Domain.Entities;
using MotoService.Domain.Enums;
using MotoService.Domain.Exceptions;
using MotoService.Domain.Interfaces;
using Xunit;

namespace tests.Unit.DeliveryTests
{
    public class DeliveryServiceTests
    {
        private readonly Mock<IDeliveryRepository> _deliveryRepositoryMock;
        private readonly Mock<IFileStorageService> _fileStorageServiceMock;
        private readonly Mock<ILogger<DeliveryService>> _mockLogger;
        private readonly DeliveryService _deliveryService;
        private readonly IMapper _mapper;

        public DeliveryServiceTests()
        {
            _deliveryRepositoryMock = new Mock<IDeliveryRepository>();
            _fileStorageServiceMock = new Mock<IFileStorageService>();
            _mockLogger = new Mock<ILogger<DeliveryService>>();
            var config = new MapperConfiguration(cfg => cfg.AddProfile<DeliveryProfile>());
            _mapper = config.CreateMapper();

            _deliveryService = new DeliveryService(
                _deliveryRepositoryMock.Object,
                _fileStorageServiceMock.Object,
                _mockLogger.Object,
                _mapper
            );
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateDelivery_WhenDataIsValid()
        {
            // Arrange
            var base64Image = Convert.ToBase64String(new byte[] { 0x89, 0x50, 0x4E, 0x47 });

            var request = new DeliveryRequestDTO
            {
                Identifier = "delivery001",
                Name = "João da Silva",
                CnhNumber = "12345678900",
                CnhType = "A",
                Cnpj = "12345678000199",
                BirthDate = new DateOnly(2000, 5, 4),
                CNHFileString = base64Image
            };

            _deliveryRepositoryMock.Setup(r => r.ExistsByCNHAsync(request.CnhNumber)).ReturnsAsync(false);
            _deliveryRepositoryMock.Setup(r => r.ExistsByCNPJAsync(request.Cnpj)).ReturnsAsync(false);
            _deliveryRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<Delivery>())).Returns(Task.CompletedTask);

            // Act
            var result = await _deliveryService.RegisterAsync(request);

            // Assert
            result.Should().NotBeNull();
            result.Name.Should().Be(request.Name);
            _deliveryRepositoryMock.Verify(r => r.CreateAsync(It.IsAny<Delivery>()), Times.Once);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenCNHAlreadyExists()
        {
            var request = new DeliveryRequestDTO
            {
                Identifier = "delivery001",
                Name = "João",
                CnhNumber = "98183228257",
                CnhType = "A",
                Cnpj = "53.421.554/0001-29",
                BirthDate = new DateOnly(2000, 5, 4),
                CNHFileString = "base64string"
            };

            _deliveryRepositoryMock.Setup(r => r.ExistsByCNHAsync(request.CnhNumber)).ReturnsAsync(true);

            var act = async () => await _deliveryService.RegisterAsync(request);

            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Já existe um entregador com este número de CNH.");
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenCNPJAlreadyExists()
        {
            var request = new DeliveryRequestDTO
            {
                Identifier = "delivery001",
                Name = "João",
                CnhNumber = "41591811988",
                CnhType = "A",
                Cnpj = "61.662.853/0001-83",
                BirthDate = new DateOnly(2000, 5, 4),
                CNHFileString = "base64string"
            };

            _deliveryRepositoryMock.Setup(r => r.ExistsByCNHAsync(request.CnhNumber)).ReturnsAsync(false);
            _deliveryRepositoryMock.Setup(r => r.ExistsByCNPJAsync(request.Cnpj)).ReturnsAsync(true);

            var act = async () => await _deliveryService.RegisterAsync(request);

            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Já existe um entregador com este CNPJ.");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnDeliveryDTO_WhenDeliveryExists()
        {
            var delivery = new Delivery("delivery001", "Ana", "11.945.466/0001-86", new DateOnly(1990, 5, 4), "84825309187", "A", "base64string");

            _deliveryRepositoryMock.Setup(r => r.GetByIdentifierAsync("delivery001")).ReturnsAsync(delivery);

            var result = await _deliveryService.GetByIdAsync("delivery001");

            result.Should().NotBeNull();
            result.Identifier.Should().Be("delivery001");
            result.Name.Should().Be("Ana");
        }

        [Fact]
        public async Task GetByIdAsync_ShouldThrow_WhenDeliveryNotFound()
        {
            _deliveryRepositoryMock.Setup(r => r.GetByIdentifierAsync("delivery999")).ReturnsAsync((Delivery?)null);

            var act = async () => await _deliveryService.GetByIdAsync("delivery999");

            await act.Should().ThrowAsync<DeliveryNotFoundException>()
                .WithMessage("Entregador não encontrado.");
        }

        [Fact]
        public async Task UploadCNHImageAsync_ShouldStoreImageAndUpdateUrl_WhenDeliveryExists()
        {
            // Arrange
            var identifier = "delivery123";
            var base64Image = Convert.ToBase64String(new byte[] { 0x89, 0x50, 0x4E, 0x47 }); 
            var delivery = new Delivery(identifier, "Paulo", "11.945.466/0001-86", new DateOnly(1990, 5, 4), "84825309187", "A", base64Image);

            var uploadedUrl = $"https://bucket/cnh/{identifier}_cnh.png";

            _deliveryRepositoryMock.Setup(r => r.GetByIdentifierAsync(identifier))
                .ReturnsAsync(delivery);

            _fileStorageServiceMock.Setup(s => s.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(uploadedUrl);

            _deliveryRepositoryMock.Setup(r => r.UpdateCNHImageUrlAsync(delivery))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _deliveryService.UploadCNHImageAsync(identifier, base64Image);

            // Assert
            result.Should().Be(uploadedUrl);
            _deliveryRepositoryMock.Verify(r => r.UpdateCNHImageUrlAsync(delivery), Times.Once);
        }

        [Fact]
        public async Task UploadCNHImageAsync_ShouldThrow_WhenDeliveryDoesNotExist()
        {
            var identifier = "delivery123";
            var base64Image = Convert.ToBase64String(new byte[] { 0x89, 0x50, 0x4E, 0x47 });

            _deliveryRepositoryMock.Setup(r => r.GetByIdentifierAsync(identifier))
                .ReturnsAsync((Delivery?)null);

            var act = async () => await _deliveryService.UploadCNHImageAsync(identifier, base64Image);

            await act.Should().ThrowAsync<DeliveryNotFoundException>()
                .WithMessage("Entregador não encontrado.");
        }
    }
}
