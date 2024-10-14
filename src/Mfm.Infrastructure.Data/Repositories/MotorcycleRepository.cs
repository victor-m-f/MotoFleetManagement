using Mfm.Domain.Entities;
using Mfm.Domain.Repositories;

namespace Mfm.Infrastructure.Data.Repositories;
internal sealed class MotorcycleRepository : IMotorcycleRepository
{
    private readonly ApplicationDbContext _context;

    public MotorcycleRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public void Add(Motorcycle motorcycle)
    {
        _context.Motorcycles.Add(motorcycle);
    }

    public Task SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
