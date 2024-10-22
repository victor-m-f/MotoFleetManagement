using Mfm.Domain.Entities;
using Mfm.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Mfm.Infrastructure.Data.Repositories;
internal sealed class RentalRepository : RepositoryBase, IRentalRepository
{
    public RentalRepository(ApplicationDbContext context) : base(context)
    {
    }

    public void Add(Rental rental)
    {
        Context.Rentals.Add(rental);
    }

    public Task<Rental?> GetByIdAsync(string id, CancellationToken cancellationToken)
    {
        return Context.Rentals.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }
}