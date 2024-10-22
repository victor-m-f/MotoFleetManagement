using Mfm.Domain.Entities.ValueObjects;
using Mfm.Domain.Exceptions;

namespace Mfm.Domain.Entities;

public sealed class DeliveryPerson
{
    public string Id { get; } = string.Empty;
    public string Name { get; } = string.Empty;
    public Cnpj Cnpj { get; } = default!;
    public DateTime DateOfBirth { get; }
    public Cnh Cnh { get; } = default!;
    public string CnhImageUrl { get; private set; } = string.Empty;

    public ICollection<Rental> Rentals { get; } = [];
    public bool IsMotorcycleDriver => Cnh.Type == Enums.CnhType.A || Cnh.Type == Enums.CnhType.AB;

    public DeliveryPerson(string id, string name, Cnpj cnpj, DateTime dateOfBirth, Cnh cnh, string cnhImageUrl)
    {
        Id = id;
        Name = name;
        Cnpj = cnpj ?? throw new ValidationException();
        DateOfBirth = dateOfBirth;
        Cnh = cnh ?? throw new ValidationException();
        CnhImageUrl = cnhImageUrl;
    }

    // This constructor is used by EF Core
    private DeliveryPerson() { }
}
