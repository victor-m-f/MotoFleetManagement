using FluentAssertions;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.Rules;
using Mfm.Domain.Entities.ValueObjects;
using Mfm.Domain.Exceptions;

namespace Mfm.Domain.UnitTests.Entities;
public sealed class MotorcycleTests
{
    [Fact]
    public void Constructor_ShouldCreateMotorcycle_WhenValidParameters()
    {
        // Arrange
        var id = "123";
        var year = DateTime.Now.Year;
        var model = "Model X";
        var licensePlate = new LicensePlate("ABC-1234");

        // Act
        var motorcycle = new Motorcycle(id, licensePlate, year, model);

        // Assert
        motorcycle.Id.Should().Be(id);
        motorcycle.Year.Should().Be(year);
        motorcycle.Model.Should().Be(model);
        motorcycle.LicensePlate.Should().Be(licensePlate);
    }

    [Fact]
    public void Constructor_ShouldThrowValidationException_WhenYearIsOutOfRange()
    {
        // Arrange
        var id = "123";
        var year = MotorcycleRules.MinYear - 1;
        var model = "Model X";
        var licensePlate = new LicensePlate("ABC-1234");

        // Act
        var motorcycle = () => new Motorcycle(id, licensePlate, year, model);

        // Assert
        motorcycle.Should().Throw<ValidationException>();
    }

    [Fact]
    public void Constructor_ShouldThrowValidationException_WhenLicensePlateIsNull()
    {
        // Arrange
        var id = "123";
        var year = DateTime.Now.Year;
        var model = "Model X";

        // Act
        var motorcycle = () => new Motorcycle(id, null!, year, model);

        // Assert
        motorcycle.Should().Throw<ValidationException>();
    }

    [Fact]
    public void UpdateLicensePlate_ShouldUpdateLicensePlate_WhenValidLicensePlate()
    {
        // Arrange
        var id = "123";
        var year = DateTime.Now.Year;
        var model = "Model X";
        var initialLicensePlate = new LicensePlate("ABC-1234");
        var motorcycle = new Motorcycle(id, initialLicensePlate, year, model);

        var newLicensePlate = new LicensePlate("XYZ-9876");

        // Act
        motorcycle.UpdateLicensePlate(newLicensePlate);

        // Assert
        motorcycle.LicensePlate.Should().Be(newLicensePlate);
    }

    [Fact]
    public void UpdateLicensePlate_ShouldThrowValidationException_WhenLicensePlateIsNull()
    {
        // Arrange
        var id = "123";
        var year = DateTime.Now.Year;
        var model = "Model X";
        var initialLicensePlate = new LicensePlate("ABC-1234");
        var motorcycle = new Motorcycle(id, initialLicensePlate, year, model);

        // Act
        Action action = () => motorcycle.UpdateLicensePlate(null!);

        // Assert
        action.Should().Throw<ValidationException>();
    }
}
