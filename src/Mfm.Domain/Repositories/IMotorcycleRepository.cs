using Mfm.Domain.Entities;

namespace Mfm.Domain.Repositories;
public interface IMotorcycleRepository : IRepository
{
    public void Add(Motorcycle motorcycle);
    public Task<bool> ExistsMotorcyleWithLicensePlateAsync(
        string licensePlate,
        CancellationToken cancellationToken);
}
