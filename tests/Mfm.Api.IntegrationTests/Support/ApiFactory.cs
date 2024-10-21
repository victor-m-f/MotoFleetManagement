using Azure.Storage.Blobs;
using Mfm.Domain.Services;
using Mfm.Infrastructure.Data;
using Mfm.Infrastructure.Messaging.Configuration;
using Mfm.Infrastructure.Storage.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.Azurite;
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

    private readonly AzuriteContainer _azurite = new AzuriteBuilder()
    .WithImage("mcr.microsoft.com/azure-storage/azurite")
    .Build();

    public async Task InitializeAsync()
    {
        var cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromMinutes(1));

        await _postgres.StartAsync(cancellationTokenSource.Token);
        await _rabbitMq.StartAsync(cancellationTokenSource.Token);
        await _azurite.StartAsync(cancellationTokenSource.Token);

        await Task.Delay(2000);
    }

    public new async Task DisposeAsync()
    {
        await Task.WhenAll(
            _postgres.StopAsync(),
            _rabbitMq.StopAsync(),
            _azurite.StopAsync());
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            ConfigureDbContext(services);
            ConfigureRabbitMq(services);
            ConfigureStorage(services);
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

    private void ConfigureStorage(IServiceCollection services)
    {
        var blobServiceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(BlobContainerClient));
        if (blobServiceDescriptor != null)
        {
            services.Remove(blobServiceDescriptor);
        }

        var storageServiceDescriptor = services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(IStorageService));
        if (storageServiceDescriptor != null)
        {
            services.Remove(storageServiceDescriptor);
        }

        services.ConfigureStorage(_azurite.GetConnectionString());
    }
}