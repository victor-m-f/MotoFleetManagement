using Mfm.Domain.Entities;

namespace Mfm.Domain.Events;
public class MotorcycleRegisteredEvent : IDomainEvent
{
    public Motorcycle Motorcycle { get; }

    public MotorcycleRegisteredEvent(Motorcycle motorcycle)
    {
        Motorcycle = motorcycle;
    }
}
