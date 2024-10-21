using Azure.Storage.Blobs;
using Mfm.Domain.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mfm.Infrastructure.Storage.Configuration;
public static class StorageConfiguration
{
    public static void ConfigureStorage(
        this IServiceCollection services,
        string? azureStorageConnectionString)
    {
        services.AddSingleton(x =>
        {
            return new BlobContainerClient(azureStorageConnectionString, "upload-images");
        });

        services.AddScoped<IStorageService, AzureStorageService>();
    }
}
