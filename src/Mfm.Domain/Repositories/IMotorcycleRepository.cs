using Mfm.Domain.Entities;

namespace Mfm.Domain.Repositories;
public interface IMotorcycleRepository : IRepository
{
    public void Add(Motorcycle motorcycle);
    public Task<bool> ExistsMotorcycleWithLicensePlateAsync(
        string licensePlate,
        CancellationToken cancellationToken);
    public Task<List<Motorcycle>> GetMotorcyclesAsync(
        string? licensePlate,
        CancellationToken cancellationToken);
    public Task<Motorcycle?> GetByIdAsync(
        string id,
        bool includeRentals = false,
        CancellationToken cancellationToken = default);
    public void Remove(Motorcycle motorcycle);
}
