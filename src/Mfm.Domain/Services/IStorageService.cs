namespace Mfm.Domain.Services;
public interface IStorageService
{
    public Task CreateBlobFileAsync(string filename, byte[] data, CancellationToken cancellationToken);

    public Task<Stream?> GetBlobFileAsync(string filename, CancellationToken cancellationToken);

    public Task DeleteBlobFileAsync(string filename, CancellationToken cancellationToken);
}