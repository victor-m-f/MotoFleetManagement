using Mfm.Domain.Entities;
using Mfm.Domain.Events.Base;

namespace Mfm.Domain.Events;
public sealed class MotorcycleRegisteredEvent : IDomainEvent
{
    public Motorcycle Motorcycle { get; }

    public MotorcycleRegisteredEvent(Motorcycle motorcycle)
    {
        Motorcycle = motorcycle;
    }
}
