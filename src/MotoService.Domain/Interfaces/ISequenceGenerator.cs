using MongoDB.Driver;

namespace MotoService.Domain.Repositories
{
    public interface ISequenceGenerator
    {
        Task<string> GetNextSequenceValueAsync(string key);
    }
}
