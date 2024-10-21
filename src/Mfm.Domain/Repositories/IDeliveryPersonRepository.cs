using Mfm.Domain.Entities;

namespace Mfm.Domain.Repositories;
public interface IDeliveryPersonRepository : IRepository
{
    public void Add(DeliveryPerson deliveryPerson);
    public Task<bool> ExistsDeliveryPersonWithCnpjAsync(
        string cnpj,
        CancellationToken cancellationToken);
    public Task<bool> ExistsDeliveryPersonWithCnhNumberAsync(
        string cnhNumber,
        CancellationToken cancellationToken);

    public Task<DeliveryPerson?> GetByIdAsync(
        string deliveryPersonId,
        CancellationToken cancellationToken);
}
