using Mfm.Domain.Entities;
using Mfm.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Mfm.Infrastructure.Data.Repositories;
internal sealed class MotorcycleRepository : RepositoryBase, IMotorcycleRepository
{

    public MotorcycleRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public void Add(Motorcycle motorcycle)
    {
        Context.Motorcycles.Add(motorcycle);
    }

    public Task<bool> ExistsMotorcycleWithLicensePlateAsync(
        string licensePlate,
        CancellationToken cancellationToken)
    {
        return Context.Motorcycles
            .AsNoTracking()
            .AnyAsync(m => m.LicensePlate.Value == licensePlate, cancellationToken);
    }

    public Task<List<Motorcycle>> GetMotorcyclesAsync(
        string? licensePlate,
        CancellationToken cancellationToken)
    {
        var query = Context.Motorcycles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(licensePlate))
        {
            query = query.Where(m => m.LicensePlate.Value == licensePlate);
        }

        return query.ToListAsync(cancellationToken);
    }

    public Task<Motorcycle?> GetByIdAsync(
        string id,
        bool includeRentals = false,
        CancellationToken cancellationToken = default)
    {
        var query = Context.Motorcycles.AsQueryable();

        if (includeRentals)
        {
            query = query.Include(m => m.Rentals);
        }

        return query.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public void Remove(Motorcycle motorcycle)
    {
        Context.Motorcycles.Remove(motorcycle);
    }
}
