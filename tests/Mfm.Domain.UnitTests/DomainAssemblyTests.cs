using FluentAssertions;
using System.Reflection;

namespace Mfm.Domain.UnitTests;
public sealed class DomainAssemblyTests
{
    [Fact]
    public void Domain_ShouldNotReferenceOtherAssemblies()
    {
        // Arrange
        var domainAssembly = Assembly.Load("Mfm.Domain");

        // Act
        var referencedAssemblies = domainAssembly.GetReferencedAssemblies();

        // Assert
        referencedAssemblies.Should().NotContain(assembly =>
            assembly.Name == "Mfm.Application" ||
            assembly.Name == "Mfm.Infrastructure.Data" ||
            assembly.Name == "Mfm.Api",
            "The Domain project should not reference Application, Infrastructure.Data, or Api projects.");
    }
}
