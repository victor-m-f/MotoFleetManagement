using FluentAssertions;
using System.Reflection;

namespace Mfm.Domain.UnitTests;
public sealed class ApplicationAssemblyTests
{
    [Fact]
    public void Application_ShouldNotReferenceOtherAssemblies()
    {
        // Arrange
        var applicationAssembly = Assembly.Load("Mfm.Application");

        // Act
        var referencedAssemblies = applicationAssembly.GetReferencedAssemblies();

        // Assert
        referencedAssemblies.Should().NotContain(assembly =>
            assembly.Name == "Mfm.Infrastructure.Data" ||
            assembly.Name == "Mfm.Api",
            "The Application project should not reference Infrastructure.Data, or Api projects.");
    }
}
