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

    public Task<bool> ExistsMotorcyleWithLicensePlateAsync(string licensePlate, CancellationToken cancellationToken)
    {
        return Context.Motorcycles
            .AsNoTracking()
            .AnyAsync(m => m.LicensePlate.Value == licensePlate, cancellationToken);
    }
}
