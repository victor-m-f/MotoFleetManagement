using Mfm.Domain.Entities;

namespace Mfm.Domain.Repositories;
public interface IRentalRepository : IRepository
{
    public void Add(Rental rental);
    public Task<Rental?> GetByIdAsync(string rentalId, CancellationToken cancellationToken);
}