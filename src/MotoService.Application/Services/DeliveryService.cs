
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MotoService.Application.DTOs;
using MotoService.Application.Interfaces;
using MotoService.Application.Mappers;
using MotoService.Domain.Entities;
using MotoService.Domain.Exceptions;
using MotoService.Domain.Interfaces;
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

        public async Task<DeliveryDTO> RegisterAsync(CreateDeliveryDTO deliveryDto)
        {
            await ValidateDelivery(deliveryDto);
            var delivery = _mapper.Map<Delivery>(deliveryDto);

            await _repository.CreateAsync(delivery);
            return _mapper.Map<DeliveryDTO>(delivery);
        }
        public async Task<string> UploadCNHImageAsync(string identifier, IFormFile cnhFile)
        {
            var delivery = await _repository.GetByIdentifierAsync(identifier) ?? throw new DeliveryNotFoundException();

            _logger.LogInformation("Alterando a CNH {CnhNumber}", identifier);

            var ext = Path.GetExtension(cnhFile.FileName).ToLower();
            if (ext != ".png" && ext != ".bmp")
                throw new InvalidFileFormatException();

            using var stream = cnhFile.OpenReadStream();
            var fileName = $"{delivery.Identifier}_cnh{ext}";
            var url = await _storage.UploadAsync(stream, fileName, cnhFile.ContentType);
            delivery.SetCnhImageUrl(url);
            await _repository.UpdateCNHImageUrlAsync(delivery);
            return delivery.CnhImageURL;
        }
        public async Task<DeliveryDTO?> GetByIdAsync(string identifier)
        {
            var delivery = await _repository.GetByIdentifierAsync(identifier) ?? throw new DeliveryNotFoundException();

            return _mapper.Map<DeliveryDTO>(delivery);
        }

        private async Task ValidateDelivery(CreateDeliveryDTO dto)
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