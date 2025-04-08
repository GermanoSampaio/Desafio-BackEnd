
using Amazon.Auth.AccessControlPolicy;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MotoService.Application.DTOs;
using MotoService.Application.Interfaces;
using MotoService.Application.Mappers;
using MotoService.Domain.Entities;
using MotoService.Domain.Exceptions;
using MotoService.Domain.Interfaces;
using MotoService.Domain.Utils;
using MotoService.Domain.Validators;

namespace MotoService.Application.Services
{
    public class DeliveryService : IDeliveryService
    {
        private readonly IDeliveryRepository _repository;
        private readonly IFileStorageService _storage;
        private readonly ILogger<DeliveryService> _logger;
        private readonly IMapper _mapper;

        public DeliveryService(IDeliveryRepository repository, IFileStorageService storage, ILogger<DeliveryService> logger, IMapper mapper)
        {
            _repository = repository;
            _storage = storage;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<DeliveryResponseDTO> RegisterAsync(DeliveryRequestDTO deliveryDto)
        {
            await ValidateDelivery(deliveryDto);
            var delivery = _mapper.Map<Delivery>(deliveryDto);

            await SaveImage(delivery);

            await _repository.CreateAsync(delivery);
            return _mapper.Map<DeliveryResponseDTO>(delivery);
        }
        public async Task<string> UploadCNHImageAsync(string identifier, string base64Image)
        {
            var delivery = await _repository.GetByIdentifierAsync(identifier) ?? throw new DeliveryNotFoundException();

            delivery.SetCnhBase64String(base64Image);

            _logger.LogInformation("Alterando a CNH {CnhNumber}", identifier);

            await SaveImage(delivery);


            await _repository.UpdateCNHImageUrlAsync(delivery);

            return delivery.CnhImageURL;
        }

        private async Task SaveImage(Delivery delivery)
        {
            var imageBytes = Convert.FromBase64String(delivery.CnhBase64String);

            var imageInfo = FileHelper.DetectImageInfo(imageBytes)
                           ?? throw new InvalidFileFormatException();

            var fileName = $"{delivery.Identifier}_cnh{imageInfo.Extension}";

           var memoryStream = new MemoryStream(imageBytes);
            var url = _storage.UploadAsync(memoryStream, fileName, imageInfo.ContentType);
                
            delivery.SetCnhImageUrl(url.Result);

        }
      
        public async Task<DeliveryResponseDTO?> GetByIdAsync(string identifier)
        {
            var delivery = await _repository.GetByIdentifierAsync(identifier) ?? throw new DeliveryNotFoundException();

            return _mapper.Map<DeliveryResponseDTO>(delivery);
        }

        private async Task ValidateDelivery(DeliveryRequestDTO dto)
        {
            if (!CnhTypeValidator.IsValid(dto.CnhType))
                throw new InvalidCnhTypeException();

            if (await _repository.ExistsByCNPJAsync(dto.Cnpj))
                throw new DuplicateCnpjException();

            if (await _repository.ExistsByCNHAsync(dto.CnhNumber))
                throw new DuplicateCnhException();
        }

    }
}