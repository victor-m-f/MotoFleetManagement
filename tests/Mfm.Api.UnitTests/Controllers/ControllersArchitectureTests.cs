using FluentAssertions;
using FluentAssertions.Common;
using FluentAssertions.Types;
using Mfm.Api.Controllers.V1;

namespace Mfm.Domain.UnitTests.Entities;
public sealed class ControllersArchitectureTests
{
    private static TypeSelector ControllersTypeSelector =>
        AllTypes
        .From(typeof(MotorcyclesController).Assembly)
        .ThatAreInNamespace("Mfm.Api.Controllers.V1");

    [Fact]
    public void DomainEntities_ShouldBeSealed()
    {
        ControllersTypeSelector
            .Should()
            .BeSealed();
    }
}
