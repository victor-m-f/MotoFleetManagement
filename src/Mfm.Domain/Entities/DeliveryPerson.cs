using Mfm.Domain.Entities.ValueObjects;
using Mfm.Domain.Exceptions;

namespace Mfm.Domain.Entities;

public sealed class DeliveryPerson
{
    public string Id { get; } = string.Empty;
    public string Name { get; } = string.Empty;
    public string Cnpj { get; } = string.Empty;
    public DateTime DateOfBirth { get; }
    public Cnh Cnh { get; } = default!;
    public string CnhImageUrl { get; private set; } = string.Empty;

    public DeliveryPerson(string id, string name, string cnpj, DateTime dateOfBirth, Cnh cnh, string cnhImageUrl)
    {
        Id = id;
        Name = name;
        Cnpj = cnpj;
        DateOfBirth = dateOfBirth;
        Cnh = cnh ?? throw new ValidationException();
        CnhImageUrl = cnhImageUrl;
    }

    // This constructor is used by EF Core
    private DeliveryPerson() { }

    public void UpdateCnhImage(string newImageUrl)
    {
        if (string.IsNullOrWhiteSpace(newImageUrl))
        {
            throw new ValidationException();
        }

        CnhImageUrl = newImageUrl;
    }
}
