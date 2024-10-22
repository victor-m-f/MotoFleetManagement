using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Mfm.Api.IntegrationTests.Features.Base;
using Mfm.Api.IntegrationTests.Support;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.Enums;
using Mfm.Domain.Entities.Rules;
using Mfm.Domain.Entities.ValueObjects;

namespace Mfm.Api.IntegrationTests.Features.Motorcycles;
public class DeleteMotorcycleTests : FeatureTestsBase
{
    public DeleteMotorcycleTests(ApiFactory apiFactory)
        : base(apiFactory)
    {
    }

    [Fact]
    public async Task ShouldDeleteMotorcycle_WhenNoRentalsExist()
    {
        // Arrange
        await SeedAsync(includeRentals: false);

        // Act
        var response = await HttpClient.DeleteAsync("/motos/motorcycle-id");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        DbContext.ChangeTracker.Clear();

        var deletedMotorcycle = await DbContext.Motorcycles.FindAsync("motorcycle-id");
        deletedMotorcycle.Should().BeNull();
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenMotorcycleHasRentals()
    {
        // Arrange
        await SeedAsync(includeRentals: true);

        // Act
        var response = await HttpClient.DeleteAsync("/motos/motorcycle-id");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Conflict);

        var existingMotorcycle = await DbContext.Motorcycles.FindAsync("motorcycle-id");
        existingMotorcycle.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenMotorcycleDoesNotExist()
    {
        // Arrange
        var nonExistentMotorcycleId = Guid.NewGuid().ToString();

        // Act
        var response = await HttpClient.DeleteAsync($"/motos/{nonExistentMotorcycleId}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain($"Motorcycle with Id '{nonExistentMotorcycleId}' was not found.");
    }

    private async Task SeedAsync(bool includeRentals)
    {
        var faker = new Faker();

        var motorcycle = new Motorcycle(
            "motorcycle-id",
            new LicensePlate(faker.Random.String2(8, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")),
            faker.Random.Int(MotorcycleRules.MinYear, DateTime.Now.Year),
            faker.Vehicle.Model());
        DbContext.Motorcycles.Add(motorcycle);

        if (includeRentals)
        {
            var deliveryPerson = new DeliveryPerson(
            "delivery-person-id",
            faker.Person.FullName,
            new Cnpj(faker.Company.Cnpj()),
            faker.Date.Past(30),
            new Cnh(
            faker.Random.String2(11, "0123456789"),
            CnhType.A),
            faker.Internet.Url());

            var rental = new Rental(
                "motorcycle-id",
                "delivery-person-id",
                RentalPlanType.SevenDays,
                DateTimeOffset.Now.AddDays(1),
                DateTimeOffset.Now.AddDays(7),
                DateTimeOffset.Now.AddDays(7),
                TimeProvider.System);

            DbContext.DeliveryPersons.Add(deliveryPerson);
            DbContext.Rentals.Add(rental);
        }

        await DbContext.SaveChangesAsync();
    }
}