using Mfm.Domain.ValueObjects;

namespace Mfm.Domain.Entities;
public sealed class Motorcycle : AggregateRoot
{
    public Guid Id { get; private set; }
    public int Year { get; private set; }
    public string Model { get; private set; } = string.Empty;
    public LicensePlate LicensePlate { get; private set; } = default!;

    public Motorcycle(LicensePlate licensePlate, int year, string model)
    {
        if (year < 1900 || year > DateTime.Now.Year)
        {
            throw new ArgumentOutOfRangeException(nameof(year), "Year must be valid.");
        }

        LicensePlate = licensePlate ?? throw new ArgumentNullException(nameof(licensePlate));
        Year = year;
        Model = model;
    }

    // This constructor is used by EF Core
    private Motorcycle() { }

    public void UpdateLicensePlate(LicensePlate newPlate)
    {
        LicensePlate = newPlate ?? throw new ArgumentNullException(nameof(newPlate));
    }
}
