using Mfm.Domain.Entities;
using Mfm.Domain.Repositories;

namespace Mfm.Infrastructure.Data.Repositories;
internal sealed class Motorcycle2024Repository : RepositoryBase, IMotorcycle2024Repository
{

    public Motorcycle2024Repository(ApplicationDbContext context)
        : base(context)
    {
    }

    public void Add(Motorcycle2024 motorcycle)
    {
        Context.Motorcycles2024.Add(motorcycle);
    }
}
