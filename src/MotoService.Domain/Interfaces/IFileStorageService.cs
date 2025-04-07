namespace MotoService.Domain.Interfaces
{
    public interface IFileStorageService
    {
        Task<string> UploadAsync(Stream stream, string fileName, string contentType);
    }
}
