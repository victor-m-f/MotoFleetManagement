using Mfm.Infrastructure.Data;
using Mfm.Infrastructure.Messaging.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Testcontainers.RabbitMq;

namespace Mfm.Api.IntegrationTests.Support;
public class ApiFactory : WebApplicationFactory<Startup>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres =
        new PostgreSqlBuilder().WithUsername("postgres")
        .WithPassword("postgres")
        .Build();

    private readonly RabbitMqContainer _rabbitMq = new RabbitMqBuilder()
        .WithImage("masstransit/rabbitmq")
        .Build();

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        await _rabbitMq.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await Task.WhenAll(
            _postgres.StopAsync(),
            _rabbitMq.StopAsync());
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            ConfigureDbContext(services);
            ConfigureRabbitMq(services);
        });

        builder.UseEnvironment("Development");
        base.ConfigureWebHost(builder);
    }

    private void ConfigureDbContext(IServiceCollection services)
    {
        var context = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(ApplicationDbContext));
        if (context != null)
        {
            services.Remove(context);
            var options = services.Where(r =>
                (r.ServiceType == typeof(DbContextOptions)) ||
                (r.ServiceType.IsGenericType && r.ServiceType.GetGenericTypeDefinition() == typeof(DbContextOptions<>)))
                .ToArray();
            foreach (var option in options)
            {
                services.Remove(option);
            }
        }

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseNpgsql(_postgres.GetConnectionString());
        });
    }

    private void ConfigureRabbitMq(IServiceCollection services)
    {
        var massTransitServices = services
            .Where(x => x.ServiceType.FullName!.StartsWith("MassTransit"))
            .ToList();

        foreach (var service in massTransitServices)
        {
            services.Remove(service);
        }

        services.ConfigureMessaging(_rabbitMq.GetConnectionString());
    }
}