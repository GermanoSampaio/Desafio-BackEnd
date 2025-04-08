using AspNetCoreRateLimit;
using MotoService.API.Configurations;
using MotoService.API.Middlewares;
using MotoService.Domain.Interfaces;
using MotoService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

var configBuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", false, true)
    .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", false, true)
    .AddEnvironmentVariables();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

SwaggerConfiguration.Configure(builder.Services);
builder.ConfigureServices();
builder.Services.AddAutoMapperSetup();
builder.Services.SetSettings(builder.Configuration);

MongoDbInitializer.RegisterSerializers();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

HeaderConfiguration.Configure(builder);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseResponseCompression();
app.UseMiddleware<ExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");
app.UseAuthorization();
app.UseIpRateLimiting();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var deliveryRepository = scope.ServiceProvider.GetRequiredService<IDeliveryRepository>();
    await deliveryRepository.EnsureIndexesAsync();

    var rentalPlanRepo = scope.ServiceProvider.GetRequiredService<IRentalPlanRepository>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    await RentalPlanSeeder.SeedAsync(rentalPlanRepo, logger, config);
}

app.Run();
