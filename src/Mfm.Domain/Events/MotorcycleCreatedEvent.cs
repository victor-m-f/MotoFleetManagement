using Mfm.Domain.Events.Base;

namespace Mfm.Domain.Events;
public sealed record MotorcycleCreatedEvent : IDomainEvent
{
    public string MotorcycleId { get; }
    public int Year { get; }
    public string Model { get; }
    public string LicensePlate { get; }

    public MotorcycleCreatedEvent(string motorcycleId, int year, string model, string licensePlate)
    {
        MotorcycleId = motorcycleId;
        Year = year;
        Model = model;
        LicensePlate = licensePlate;
    }
}
