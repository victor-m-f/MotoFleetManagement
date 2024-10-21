using Mfm.Domain.Repositories;
using Mfm.Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mfm.Infrastructure.Data.Configuration;
public static class DataConfiguration
{
    public static void ConfigureData(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Database");

        services.AddDbContext<ApplicationDbContext>(
            options => options.UseNpgsql(connectionString));

        services.AddScoped<IMotorcycleRepository, MotorcycleRepository>();
        services.AddScoped<IMotorcycle2024Repository, Motorcycle2024Repository>();
        services.AddScoped<IDeliveryPersonRepository, DeliveryPersonRepository>();
    }

    public static void ApplyMigrations(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        dbContext.Database.Migrate();
    }
}
