using FluentAssertions;
using FluentAssertions.Types;
using Mfm.Domain.Entities.ValueObjects;

namespace Mfm.Domain.UnitTests.ValueObjects;
public sealed class ValueObjectsArchitectureTests
{
    private static TypeSelector ValueObjectTypeSelector =>
        AllTypes
        .From(typeof(LicensePlate).Assembly)
        .ThatAreInNamespace("Mfm.Domain.Entities.ValueObjects")
        .ThatSatisfy(x => !x.Name.Contains("AnonymousType") && !x.Name.Contains("<>"));

    [Fact]
    public void ValueObjects_ShouldBeSealed()
    {
        ValueObjectTypeSelector
            .Should()
            .BeSealed();
    }

    [Fact]
    public void ValueObjects_ShouldNotHaveDefaultConstructor()
    {
        foreach (var type in ValueObjectTypeSelector.ToList())
        {
            var act = () =>
                type.Should()
                .NotHaveDefaultConstructor();

            act.Should().NotThrow($"Expected {type.Name} to not have a default constructor.");
        }
    }

    [Fact]
    public void ValueObjects_ShouldImplementIEquatable()
    {
        var valueObjectTypes = ValueObjectTypeSelector.ToList();

        foreach (var type in valueObjectTypes)
        {
            var equatableInterface = typeof(IEquatable<>).MakeGenericType(type);
            var act = () => type
            .GetInterfaces()
            .Should()
            .Contain(equatableInterface, $"Type {type.Name} should implement IEquatable<{type.Name}>");

            act.Should().NotThrow();
        }
    }
}
