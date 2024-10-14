using Mfm.Domain.Entities;

namespace Mfm.Domain.Repositories;
public interface IMotorcycleRepository
{
    public void Add(Motorcycle motorcycle);
    public Task SaveChangesAsync();
}
