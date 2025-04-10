using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Util;
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

            _logger.LogInformation("BucketName: {Bucket}", _settings.BucketName);
            _logger.LogInformation("AccessKey {AccessKey}", _settings.AccessKey);
            _logger.LogInformation("SecretKey {SecretKey}", _settings.SecretKey);
            _logger.LogInformation("ServiceURL {ServiceURL}", _settings.ServiceURL);
            _logger.LogInformation("Url {EndpointUrl}", _settings.EndpointUrl);

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
            if (!await AmazonS3Util.DoesS3BucketExistV2Async(_s3Client, _settings.BucketName))
            {
                _logger.LogError("Bucket {BucketName} não existe!", _settings.BucketName);
                throw new Exception("Bucket não encontrado.");
            }

            await EnsureBucketExistsAsync();

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

        public async Task EnsureBucketExistsAsync()
        {
            _logger.LogInformation("BucketName: {Bucket}", _settings.BucketName);
            _logger.LogInformation("AccessKey {AccessKey}", _settings.AccessKey);
            _logger.LogInformation("SecretKey {SecretKey}", _settings.SecretKey);
            _logger.LogInformation("ServiceURL {ServiceURL}", _settings.ServiceURL);
            _logger.LogInformation("Url {EndpointUrl}", _settings.EndpointUrl);

            try
            {
                var listBucketsResponse = await _s3Client.ListBucketsAsync();
              
                if (listBucketsResponse.Buckets != null && !listBucketsResponse.Buckets.Any(b => b.BucketName == _settings.BucketName))
                {
                        await _s3Client.PutBucketAsync(new PutBucketRequest
                    {
                        BucketName = _settings.BucketName,
                        UseClientRegion = true
                    });
                }
            }
            catch (AmazonS3Exception ex)
            {
                throw new Exception("Erro ao verificar ou criar o bucket no S3.", ex);
            }
        }
    }
}