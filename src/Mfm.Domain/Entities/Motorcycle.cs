using Mfm.Domain.Entities.Base;
using Mfm.Domain.Entities.Rules;
using Mfm.Domain.Entities.ValueObjects;
using Mfm.Domain.Exceptions;

namespace Mfm.Domain.Entities;
public sealed class Motorcycle : AggregateRoot
{
    public string Id { get; } = string.Empty;
    public int Year { get; }
    public string Model { get; } = string.Empty;
    public LicensePlate LicensePlate { get; private set; } = default!;

    public Motorcycle(string id, LicensePlate licensePlate, int year, string model)
    {
        if (year < MotorcycleRules.MinYear || year > DateTime.Now.Year)
        {
            throw new ValidationException();
        }

        Id = id;
        LicensePlate = licensePlate ?? throw new ValidationException();
        Year = year;
        Model = model;
    }

    // This constructor is used by EF Core
    private Motorcycle() { }

    public void UpdateLicensePlate(LicensePlate newPlate)
    {
        LicensePlate = newPlate ?? throw new ValidationException();
    }
}
