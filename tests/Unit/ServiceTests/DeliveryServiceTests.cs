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
            var request = new CreateDeliveryDTO
            {
                Name = "João da Silva",
                CnhNumber = "12345678900",
                CnhType = CnhType.A,
                Cnpj = "12345678000199",
                BirthDate = DateTime.Now.AddYears(-25)
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
            var request = new CreateDeliveryDTO
            {
                Name = "João",
                CnhNumber = "98183228257",
                CnhType = CnhType.A,
                Cnpj = "53.421.554/0001-29",
                BirthDate = DateTime.Now.AddYears(-25)
            };

            _deliveryRepositoryMock.Setup(r => r.ExistsByCNHAsync(request.CnhNumber)).ReturnsAsync(true);

            var act = async () => await _deliveryService.RegisterAsync(request);

            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Já existe um entregador com este número de CNH.");
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrow_WhenCNPJAlreadyExists()
        {
            var request = new CreateDeliveryDTO
            {
                Name = "João",
                CnhNumber = "41591811988",
                CnhType = CnhType.A,
                Cnpj = "61.662.853/0001-83",
                BirthDate = DateTime.Now.AddYears(-25)
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
            var delivery = new Delivery("Ana", "11.945.466/0001-86", new DateTime(1990, 5, 4), "84825309187", CnhType.A);
            delivery.SetIdentifier("delivery001");

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
            var delivery = new Delivery("Paulo", "11.945.466/0001-86", new DateTime(1990, 5, 4), "84825309187", CnhType.A);
            delivery.SetIdentifier("delivery123");

            var fileMock = new Mock<IFormFile>();
            var fileName = "cnh.png";
            var stream = new MemoryStream();
            var writer = new StreamWriter(stream);
            writer.Write("test file");
            writer.Flush();
            stream.Position = 0;

            fileMock.Setup(f => f.FileName).Returns(fileName);
            fileMock.Setup(f => f.OpenReadStream()).Returns(stream);
            fileMock.Setup(f => f.Length).Returns(stream.Length);
            fileMock.Setup(f => f.ContentType).Returns("image/png");

            var uploadedUrl = $"https://bucket/cnh/{fileName}";

            _deliveryRepositoryMock.Setup(r => r.GetByIdentifierAsync(delivery.Identifier))
                .ReturnsAsync(delivery);

            _fileStorageServiceMock.Setup(s => s.UploadAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(uploadedUrl);

            _deliveryRepositoryMock.Setup(r => r.UpdateCNHImageUrlAsync(delivery))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _deliveryService.UploadCNHImageAsync(delivery.Identifier, fileMock.Object);

            // Assert
            result.Should().Be(uploadedUrl);
            _deliveryRepositoryMock.Verify(r => r.UpdateCNHImageUrlAsync(delivery), Times.Once);
        }

        [Fact]
        public async Task UploadCNHImageAsync_ShouldThrow_WhenDeliveryDoesNotExist()
        {
            _deliveryRepositoryMock.Setup(r => r.GetByIdentifierAsync("delivery123"))
                .ReturnsAsync((Delivery?)null);

            var fileMock = new Mock<IFormFile>();

            var act = async () => await _deliveryService.UploadCNHImageAsync("delivery123", fileMock.Object);

            await act.Should().ThrowAsync<DeliveryNotFoundException>()
                .WithMessage("Entregador não encontrado.");
        }
    }
}
