using FluentAssertions;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.ValueObjects;
using NSubstitute;

namespace Mfm.Domain.UnitTests.Entities;
public sealed class Motorcycle2024Tests
{
    [Fact]
    public void Constructor_ShouldCreateMotorcycle_WhenValidParameters()
    {
        // Arrange
        var id = "123";
        var year = DateTime.Now.Year;
        var model = "Model X";
        var licensePlate = new LicensePlate("ABC-1234");
        var timeProvider = Substitute.For<TimeProvider>();
        var expectedCreationDate = DateTimeOffset.UtcNow;
        timeProvider.GetUtcNow().Returns(expectedCreationDate);

        // Act
        var motorcycle = new Motorcycle2024(id, year, licensePlate, model, timeProvider);

        // Assert
        motorcycle.Id.Should().Be(id);
        motorcycle.Year.Should().Be(year);
        motorcycle.Model.Should().Be(model);
        motorcycle.LicensePlate.Should().Be(licensePlate);
    }
}
