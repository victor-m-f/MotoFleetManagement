using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Types;
using Mfm.Domain.Entities;

namespace Mfm.Domain.UnitTests.Entities;
public sealed class EntitiesArchitectureTests
{
    private static TypeSelector EntitiesTypeSelector =>
        AllTypes
        .From(typeof(Motorcycle).Assembly)
        .ThatAreInNamespace("Mfm.Domain.Entities");

    [Fact]
    public void DomainEntities_ShouldBeSealed()
    {
        EntitiesTypeSelector
            .Should()
            .BeSealed();
    }

    [Fact]
    public void DomainEntities_ShouldHaveADefaultPrivateConstructor()
    {
        foreach (var type in EntitiesTypeSelector.ToList())
        {
            var act = () =>
                type.Should()
                .HaveDefaultConstructor()
                .Which
                .Should()
                .HaveAccessModifier(CSharpAccessModifier.Private);

            act.Should().NotThrow($"Expected {type.Name} to have a default private constructor.");
        }
    }
}
