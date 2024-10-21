using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.Enums;
using Mfm.Domain.Entities.ValueObjects;

namespace Mfm.Domain.UnitTests.Entities;
public sealed class DeliveryPersonTests
{
    private readonly Faker<DeliveryPerson> _deliveryPersonFaker;
    private readonly Faker<Cnh> _cnhFaker;
    private readonly Faker<Cnpj> _cnpjFaker;

    public DeliveryPersonTests()
    {
        _cnhFaker = new Faker<Cnh>()
            .CustomInstantiator(x => new Cnh(
                x.Random.String2(11, "0123456789"),
                x.PickRandom<CnhType>()));

        _cnpjFaker = new Faker<Cnpj>()
            .CustomInstantiator(x => new Cnpj(
                x.Company.Cnpj()));

        _deliveryPersonFaker = new Faker<DeliveryPerson>()
            .CustomInstantiator(x => new DeliveryPerson(
                x.Random.Guid().ToString(),
                x.Person.FullName,
                _cnpjFaker.Generate(),
                x.Date.Past(30, DateTime.Now.AddYears(-18)),
                _cnhFaker.Generate(),
                x.Internet.Url()));
    }

    [Fact]
    public void Constructor_ShouldCreateDeliveryPerson_WhenValidParameters()
    {
        // Act
        var deliveryPerson = _deliveryPersonFaker.Generate();

        // Assert
        deliveryPerson.Id.Should().NotBeNullOrWhiteSpace();
        deliveryPerson.Name.Should().NotBeNullOrWhiteSpace();
        deliveryPerson.Cnpj.Should().NotBeNull();
        deliveryPerson.Cnh.Should().NotBeNull();
        deliveryPerson.CnhImageUrl.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void Constructor_ShouldThrowValidationException_WhenCnhIsNull()
    {
        // Arrange
        var faker = new Faker();
        var action = () => new DeliveryPerson(
            faker.Random.Guid().ToString(),
            faker.Person.FullName,
            _cnpjFaker.Generate(),
            faker.Date.Past(30, DateTime.Now.AddYears(-18)),
            null!,
            faker.Internet.Url());

        // Act & Assert
        action.Should().Throw<Exceptions.ValidationException>();
    }

    [Fact]
    public void Constructor_ShouldThrowValidationException_WhenCnpjIsInvalid()
    {
        // Arrange
        var faker = new Faker();

        // Act
        var action = () => new DeliveryPerson(
            faker.Random.Guid().ToString(),
            faker.Person.FullName,
            new Cnpj("12345678000100"),
            faker.Date.Past(30, DateTime.Now.AddYears(-18)),
            new Cnh("12345678910", CnhType.A),
            faker.Internet.Url());

        // Assert
        action.Should().Throw<Exceptions.ValidationException>();
    }
}
