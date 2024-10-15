using Mfm.Domain.Entities.ValueObjects;

namespace Mfm.Domain.Entities;
public sealed class Motorcycle2024
{
    public string Id { get; } = string.Empty;
    public int Year { get; }
    public string Model { get; } = string.Empty;
    public LicensePlate LicensePlate { get; private set; } = default!;
    public DateTimeOffset CreationDate { get; }

    public Motorcycle2024(string id, int year, LicensePlate licensePlate, string model, TimeProvider timeProvider)
    {

        Id = id;
        Year = year;
        LicensePlate = licensePlate;
        Model = model;
        CreationDate = timeProvider.GetUtcNow();
    }

    // This constructor is used by EF Core
    private Motorcycle2024() { }
}
