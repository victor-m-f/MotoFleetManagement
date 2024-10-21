using Azure.Storage.Blobs;
using Mfm.Domain.Services;
using Mfm.Infrastructure.Data;
using Mfm.Infrastructure.Messaging.Configuration;
using Mfm.Infrastructure.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

    private readonly ILoggerFactory _loggerFactory;
    private readonly ILogger<ApiFactory> _logger;

    private int _azuriteBlobPort;
    private int _rabbitMqPort;

    public ApiFactory()
    {
        _loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.AddConsole();
            builder.SetMinimumLevel(LogLevel.Debug);
        });

        _logger = _loggerFactory.CreateLogger<ApiFactory>();
    }

    public async Task InitializeAsync()
    {
        _logger.LogInformation("Starting containers...");

        try
        {
            await _postgres.StartAsync();
            _logger.LogInformation("PostgreSQL container started.");

            await _rabbitMq.StartAsync();
            _logger.LogInformation("RabbitMQ container started.");

            await _azurite.StartAsync();
            _logger.LogInformation("Azurite container started.");

            _azuriteBlobPort = _azurite.GetMappedPublicPort(10000);
            _logger.LogInformation($"Azurite Blob Port: {_azuriteBlobPort}");

            _rabbitMqPort = _rabbitMq.GetMappedPublicPort(5672);
            _logger.LogInformation($"RabbitMQ Port: {_rabbitMqPort}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during container startup");
            throw;
        }

        _logger.LogInformation("Containers started. Delaying to allow services to initialize...");
        await Task.Delay(5000);
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
        try
        {
            _logger.LogInformation("Configuring storage...");

            var storageDescriptors = services.Where(
                d => d.ServiceType == typeof(BlobContainerClient) ||
                     d.ServiceType == typeof(IStorageService))
                .ToList();

            foreach (var descriptor in storageDescriptors)
            {
                services.Remove(descriptor);
            }

            var azuriteIpAddress = _azurite.IpAddress;

            var azureStorageConnectionString = $"DefaultEndpointsProtocol=http;" +
                $"AccountName=devstoreaccount1;" +
                $"AccountKey=Eby8vdM02xNoGFGeGGb0GF2h3/WE3bN/29tBT//==;" +
                $"BlobEndpoint=http://{azuriteIpAddress}:{_azuriteBlobPort}/devstoreaccount1;";

            _logger.LogInformation($"Azurite Connection String: {azureStorageConnectionString}");

            services.AddSingleton(x =>
            {
                var blobContainerClient = new BlobContainerClient(_azurite.GetConnectionString(), "upload-images");
                _logger.LogInformation("BlobContainerClient created.");
                return blobContainerClient;
            });

            services.AddScoped<IStorageService, AzureStorageService>();
            _logger.LogInformation("Storage configured.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during storage configuration");
            throw;
        }
    }
}