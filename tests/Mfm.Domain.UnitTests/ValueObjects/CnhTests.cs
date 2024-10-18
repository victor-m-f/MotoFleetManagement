using Bogus;
using FluentAssertions;
using Mfm.Domain.Entities.Enums;
using Mfm.Domain.Entities.ValueObjects;

namespace Mfm.Domain.UnitTests.ValueObjects;
public sealed class CnhTests
{
    private readonly Faker _faker;

    public CnhTests()
    {
        _faker = new Faker();
    }

    [Fact]
    public void Constructor_ShouldCreateCnh_WhenValidParameters()
    {
        // Arrange
        var number = _faker.Random.String2(11, "0123456789");
        var type = CnhType.A;

        // Act
        var cnh = new Cnh(number, type);

        // Assert
        cnh.Number.Should().Be(number);
        cnh.Type.Should().Be(type);
    }

    [Fact]
    public void Constructor_ShouldThrowValidationException_WhenNumberIsInvalid()
    {
        // Arrange
        var invalidNumber = "123";

        // Act
        var action = () => new Cnh(invalidNumber, CnhType.A);

        // Assert
        action.Should().Throw<Exceptions.ValidationException>();
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenCnhsAreEqual()
    {
        // Arrange
        var number = "12345678910";
        var cnh1 = new Cnh(number, CnhType.A);
        var cnh2 = new Cnh(number, CnhType.A);

        // Act & Assert
        cnh1.Should().Be(cnh2);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenCnhsAreDifferent()
    {
        // Arrange
        var cnh1 = new Cnh("12345678910", CnhType.A);
        var cnh2 = new Cnh("98765432100", CnhType.B);

        // Act & Assert
        cnh1.Should().NotBe(cnh2);
    }
}
