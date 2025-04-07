using MongoDB.Driver;

namespace MotoService.Domain.Repositories
{
    public interface ISequenceGenerator
    {
        Task<long> GetNextSequenceValueAsync(string key);
    }
}
