using FluentAssertions;
using FluentAssertions.Types;
using Mfm.Domain.Events;
using Mfm.Domain.Events.Base;

namespace Mfm.Domain.UnitTests.Entities;
public sealed class EventsArchitectureTests
{
    private static TypeSelector EventsTypeSelector =>
        AllTypes
        .From(typeof(MotorcycleRegisteredEvent).Assembly)
        .ThatAreInNamespace("Mfm.Domain.Events");

    [Fact]
    public void Events_ShouldBeSealed()
    {
        EventsTypeSelector
            .Should()
            .BeSealed();
    }

    [Fact]
    public void Events_ShouldNotHaveDefaultConstructor()
    {
        foreach (var type in EventsTypeSelector.ToList())
        {
            var act = () =>
                type.Should()
                .NotHaveDefaultConstructor();

            act.Should().NotThrow($"Expected {type.Name} to not have a default constructor.");
        }
    }

    [Fact]
    public void Events_ShouldImplementIEquatable()
    {
        var eventTypes = EventsTypeSelector.ToList();

        foreach (var type in eventTypes)
        {
            var act = () => type
            .Should()
            .Implement<IDomainEvent>();

            act.Should().NotThrow($"Expected {type.Name} to implement {nameof(IDomainEvent)} interface.");
        }
    }
}
