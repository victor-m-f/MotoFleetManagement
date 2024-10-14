using Mfm.Domain.Entities;
using Mfm.Domain.Events.Base;

namespace Mfm.Domain.Events;
public sealed class MotorcycleCreatedEvent : IDomainEvent
{
    public Motorcycle Motorcycle { get; }

    public MotorcycleCreatedEvent(Motorcycle motorcycle)
    {
        Motorcycle = motorcycle;
    }
}
