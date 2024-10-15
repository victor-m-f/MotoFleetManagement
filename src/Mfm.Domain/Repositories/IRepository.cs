namespace Mfm.Domain.Repositories;
public interface IRepository
{
    public Task SaveChangesAsync(CancellationToken cancellationToken);
}
