using Bogus;
using FluentAssertions;
using Mfm.Domain.Entities.ValueObjects;

namespace Mfm.Domain.UnitTests.ValueObjects;
public sealed class CnpjTests
{
    [Theory]
    [InlineData("12345678000195", true)]
    [InlineData("82536830000100", true)]
    [InlineData("00000000000000", false)]
    [InlineData("123", false)]
    [InlineData("11111111111111", false)]
    [InlineData("12345678000100", false)]
    public void Constructor_ShouldValidateCnpjCorrectly(string input, bool isValid)
    {
        // Act
        var action = () => new Cnpj(input);

        // Assert
        if (isValid)
        {
            action.Should().NotThrow<Exceptions.ValidationException>();
        }
        else
        {
            action.Should().Throw<Exceptions.ValidationException>();
        }
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenCnpjsAreEqual()
    {
        // Arrange
        var cnpj1 = new Cnpj("12345678000195");
        var cnpj2 = new Cnpj("12345678000195");

        // Act & Assert
        cnpj1.Should().Be(cnpj2);
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenCnpjsAreDifferent()
    {
        // Arrange
        var cnpj1 = new Cnpj("12345678000195");
        var cnpj2 = new Cnpj("82536830000100");

        // Act & Assert
        cnpj1.Should().NotBe(cnpj2);
    }
}
