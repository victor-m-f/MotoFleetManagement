using FluentAssertions;
using FluentAssertions.Types;
using Mfm.Api.Controllers.V1;

namespace Mfm.Api.UnitTests.Controllers;
public sealed class ControllersArchitectureTests
{
    private static TypeSelector ControllersTypeSelector =>
        AllTypes
        .From(typeof(MotorcyclesController).Assembly)
        .ThatAreInNamespace("Mfm.Api.Controllers.V1");

    [Fact]
    public void Controllers_ShouldBeSealed()
    {
        ControllersTypeSelector
            .Should()
            .BeSealed();
    }
}
