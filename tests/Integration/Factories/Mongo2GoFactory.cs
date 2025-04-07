using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Mongo2Go;
using MongoDB.Driver;
using Moq;
using MotoService.Infrastructure.Persistence.Contexts;
using MotoService.Infrastructure.Persistence.Settings;

namespace MotoService.Tests.Integration.Factories
{
    public class MongoDbContextFactory
    {
        public static async Task<(MongoDbRunner, MongoClient, IClientSessionHandle, MongoDbContext)> CreateAsync()
        {
            var mongoRunner = MongoDbRunner.Start();
            var client = new MongoClient(mongoRunner.ConnectionString);

            var mongoDbSettingsMock = new Mock<IOptions<MongoDbSettings>>();
            mongoDbSettingsMock.Setup(m => m.Value).Returns(new MongoDbSettings
            {
                ConnectionString = mongoRunner.ConnectionString,
                DatabaseName = "TestDb"
            });

            var configurationMock = new Mock<IConfiguration>();
            var mongoDbContext = new MongoDbContext(configurationMock.Object, mongoDbSettingsMock.Object);

            var session = await client.StartSessionAsync();

            return (mongoRunner, client, session, mongoDbContext);
        }
    }
}