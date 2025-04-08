using MotoService.Application.Mappers;

namespace MotoService.API.Configurations
{
    public static class AutoMapperConfiguration
    {
        public static void AddAutoMapperSetup(this IServiceCollection services)
        {
            services.AddAutoMapper(
                 typeof(MotorcycleProfile).Assembly,
                 typeof(DeliveryProfile).Assembly,
                 typeof(RentalProfile).Assembly
             );

        }
    }
}
