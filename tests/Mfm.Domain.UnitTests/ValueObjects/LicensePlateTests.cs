using Bogus;
using FluentAssertions;
using Mfm.Domain.Entities.Rules;
using Mfm.Domain.Entities.ValueObjects;

namespace Mfm.Domain.UnitTests.ValueObjects;
public sealed class LicensePlateTests
{
    private readonly Faker _faker;

    public LicensePlateTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void Constructor_ShouldCreateLicensePlate_WhenValidValueIsProvided()
    {
        // Arrange
        var validPlate = _faker.Random.String2(MotorcycleRules.LicensePlateMaxLength);

        // Act
        var licensePlate = new LicensePlate(validPlate);

        // Assert
        licensePlate.Value.Should().Be(validPlate);
    }

    [Fact]
    public void Constructor_ShouldThrowValidationException_WhenValueIsNullOrEmpty()
    {
        // Arrange
        var action = () => new LicensePlate("");

        // Act & Assert
        action.Should().Throw<Exceptions.ValidationException>();
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenLicensePlatesAreEqual()
    {
        // Arrange
        var plate = "ABC-1234";
        var licensePlate1 = new LicensePlate(plate);
        var licensePlate2 = new LicensePlate(plate);

        // Act & Assert
        licensePlate1.Should().Be(licensePlate2);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenLicensePlatesAreDifferent()
    {
        // Arrange
        var licensePlate1 = new LicensePlate("ABC-1234");
        var licensePlate2 = new LicensePlate("XYZ-5678");

        // Act & Assert
        licensePlate1.Should().NotBe(licensePlate2);
    }
}