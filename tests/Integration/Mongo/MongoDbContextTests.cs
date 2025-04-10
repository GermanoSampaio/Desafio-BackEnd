using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MotoService.Infrastructure.Persistence.Contexts;
using MotoService.Infrastructure.Persistence.Settings;

namespace MotoService.Tests.Integration.Mongo
{
    public class MongoDbContextTests
    {
        private readonly MongoDbContext _context;

        public MongoDbContextTests()
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            var settings = Options.Create(configuration.GetSection("MongoDbSettings").Get<MongoDbSettings>());
            var logger = new LoggerFactory().CreateLogger<MongoDbContext>();

            _context = new MongoDbContext(configuration, settings);
        }

        [Fact]
        public void ShouldConnectToDatabase()
        {
            var databaseName = _context.Client.GetDatabase("MotoServiceDb").DatabaseNamespace.DatabaseName;
            Assert.Equal("MotoServiceDb", databaseName);
        }
    }
}