using MotoService.Domain.Interfaces;

namespace MotoService.Tests.Integration.Mocks
{
    public class FileStorageServiceMock : IFileStorageService
    {
        private readonly Dictionary<string, byte[]> _storage = new();
        public Task<string> UploadAsync(Stream fileStream, string fileName, string contentType)
        {
            using var ms = new MemoryStream();
            fileStream.CopyTo(ms);
            _storage[fileName] = ms.ToArray();
            return Task.FromResult($"https://fake.storage/{fileName}");
        }

        public Task<Stream> DownloadAsync(string fileName)
        {
            return Task.FromResult<Stream>(new MemoryStream(_storage[fileName]));
        }
    }
}
