using Mfm.Domain.Entities;
using Mfm.Domain.Repositories;

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
}
