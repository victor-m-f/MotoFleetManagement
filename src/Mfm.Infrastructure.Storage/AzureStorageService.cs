using Azure.Storage.Blobs;
using Mfm.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Mfm.Infrastructure.Storage;
internal sealed class AzureStorageService : IStorageService
{
    private const string BlobExistsMessage = "Blob '{Filename}' already exists.";
    private const string BlobUploadSuccessMessage = "Blob '{Filename}' uploaded successfully.";
    private const string BlobUploadErrorMessage = "Failed to upload blob '{Filename}'.";
    private const string BlobRetrieveSuccessMessage = "Blob '{Filename}' retrieved successfully.";
    private const string BlobNotFoundMessage = "Blob '{Filename}' not found.";
    private const string BlobDeleteSuccessMessage = "Blob '{Filename}' deleted successfully.";
    private const string BlobDeleteErrorMessage = "Failed to delete blob '{Filename}'.";

    private readonly ILogger<AzureStorageService> _logger;
    private readonly BlobContainerClient _blobClient;

    public AzureStorageService(ILogger<AzureStorageService> logger, BlobContainerClient blobClient)
    {
        _logger = logger;
        _blobClient = blobClient;
        EnsureContainerExistsAsync().GetAwaiter().GetResult();
    }

    public async Task CreateBlobFileAsync(
        string filename,
        byte[] data,
        CancellationToken cancellationToken)
    {
        var blob = GetBlob(filename);
        if (blob.Exists(cancellationToken))
        {
            await DeleteBlobFileAsync(filename, cancellationToken);
        }

        try
        {
            await using var memoryStream = new MemoryStream(data, false);
            await _blobClient.UploadBlobAsync(filename, memoryStream, cancellationToken);
            _logger.LogInformation(BlobUploadSuccessMessage, filename);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, BlobUploadErrorMessage, filename);
            throw;
        }
    }

    public async Task<Stream?> GetBlobFileAsync(
        string filename,
        CancellationToken cancellationToken)
    {
        var blob = GetBlob(filename);
        if (!blob.Exists(cancellationToken))
        {
            _logger.LogWarning(BlobNotFoundMessage, filename);
            return null;
        }

        try
        {
            var blobContent = await blob.DownloadContentAsync(cancellationToken);
            _logger.LogInformation(BlobRetrieveSuccessMessage, filename);
            return blobContent.Value.Content.ToStream();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, "Error retrieving blob '{Filename}'.", filename);
            throw;
        }
    }

    public async Task DeleteBlobFileAsync(string filename, CancellationToken cancellationToken)
    {
        try
        {
            var deleted = await _blobClient.DeleteBlobIfExistsAsync(filename, cancellationToken: cancellationToken);
            if (deleted)
            {
                _logger.LogInformation(BlobDeleteSuccessMessage, filename);
            }
            else
            {
                _logger.LogWarning(BlobNotFoundMessage, filename);
            }
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, BlobDeleteErrorMessage, filename);
            throw;
        }
    }

    private async Task EnsureContainerExistsAsync()
    {
        if (!await _blobClient.ExistsAsync())
        {
            await _blobClient.CreateIfNotExistsAsync();
            _logger.LogInformation("Blob container '{ContainerName}' created.", _blobClient.Name);
        }
    }

    private BlobClient GetBlob(string filename)
    {
        return _blobClient.GetBlobClient(filename);
    }
}