using Mfm.Domain.Entities;
using Mfm.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Mfm.Infrastructure.Data.Repositories;
internal sealed class DeliveryPersonRepository : RepositoryBase, IDeliveryPersonRepository
{

    public DeliveryPersonRepository(ApplicationDbContext context)
        : base(context)
    {
    }

    public void Add(DeliveryPerson deliveryPerson)
    {
        Context.DeliveryPersons.Add(deliveryPerson);
    }

    public Task<bool> ExistsDeliveryPersonWithCnpjAsync(
        string cnpj,
        CancellationToken cancellationToken)
    {
        return Context.DeliveryPersons
            .AsNoTracking()
            .AnyAsync(m => m.Cnpj.Value == cnpj, cancellationToken);
    }

    public Task<bool> ExistsDeliveryPersonWithCnhNumberAsync(
        string cnhNumber,
        CancellationToken cancellationToken)
    {
        return Context.DeliveryPersons
            .AsNoTracking()
            .AnyAsync(m => m.Cnh.Number == cnhNumber, cancellationToken);
    }
}
