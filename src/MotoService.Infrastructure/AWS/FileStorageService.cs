using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MotoService.Domain.Interfaces;
using MotoService.Infrastructure.AWS.Settings;
using Polly;
using Polly.Retry;

namespace MotoService.Infrastructure.AWS
{
    public class FileStorageService : IFileStorageService
    {
        private readonly IAmazonS3 _s3Client;
        private readonly AWSSettings _settings;
        private readonly ILogger<FileStorageService> _logger;
        private readonly AsyncRetryPolicy _retryPolicy;

        public FileStorageService(
            IAmazonS3 s3Client,
            IConfiguration configuration,
            IOptions<AWSSettings> settings,
            ILogger<FileStorageService> logger)
        {
            _s3Client = s3Client;
            _settings = settings.Value;
            _logger = logger;

            _retryPolicy = Policy
                .Handle<AmazonS3Exception>() 
                .Or<HttpRequestException>()  
                .Or<TimeoutException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogWarning(exception,
                            "Tentativa {RetryCount} de upload falhou. Aguardando {Delay}s para nova tentativa...",
                            retryCount, timeSpan.TotalSeconds);
                    });
        }

        public async Task<string> UploadAsync(Stream originalStream, string fileName, string contentType)
        {
            var key = $"cnh/{fileName}";

            var streamBytes = await GetBytesAsync(originalStream); 
            var request = new PutObjectRequest
            {
                BucketName = _settings.BucketName,
                Key = key,
                ContentType = contentType,
                CannedACL = S3CannedACL.PublicRead
            };

            await _retryPolicy.ExecuteAsync(() =>
            {
                var retryStream = new MemoryStream(streamBytes);
                request.InputStream = retryStream;

                return _s3Client.PutObjectAsync(request);
            });

            return $"{_settings.EndpointUrl}/{_settings.BucketName}/{key}";
        }

        private static async Task<byte[]> GetBytesAsync(Stream stream)
        {
            using var memory = new MemoryStream();
            stream.Position = 0; 
            await stream.CopyToAsync(memory);
            return memory.ToArray();
        }
    }
}