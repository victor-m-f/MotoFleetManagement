using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Mfm.Api.IntegrationTests.Features.Base;
using Mfm.Api.IntegrationTests.Support;
using Mfm.Application.Dtos.Rentals;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.Enums;
using Mfm.Domain.Entities.Rules;
using Mfm.Domain.Entities.ValueObjects;
using Mfm.Domain.Services;

namespace Mfm.Api.IntegrationTests.Features.Rentals;
public class GetRentalByIdTests : FeatureTestsBase
{
    public GetRentalByIdTests(ApiFactory apiFactory)
        : base(apiFactory)
    {
    }

    [Fact]
    public async Task ShouldReturnRental_WhenRentalExists()
    {
        // Arrange
        var rental = await SeedRentalAsync();

        // Act
        var response = await HttpClient.GetAsync($"/locacao/{rental.Id}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        var returnedRental = JsonHelper.Deserialize<RentalDto>(responseContent);

        returnedRental.Should().NotBeNull();
        returnedRental!.Id.Should().Be(rental.Id);
        returnedRental.DailyRate.Should().Be(RentalPlan.GetPlan(rental.PlanType).DailyRate);
        returnedRental.DeliveryPersonId.Should().Be(rental.DeliveryPersonId);
        returnedRental.MotorcycleId.Should().Be(rental.MotorcycleId);
        returnedRental.StartDate.Should().Be(rental.Period.StartDate);
        returnedRental.EndDate.Should().Be(rental.Period.EndDate);
        returnedRental.ExpectedEndDate.Should().Be(rental.Period.ExpectedEndDate);
        returnedRental.ReturnDate.Should().Be(rental.ReturnDate);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenRentalDoesNotExist()
    {
        // Act
        var response = await HttpClient.GetAsync("/locacao/non-existing-id");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    private async Task<Rental> SeedRentalAsync()
    {
        var faker = new Faker();

        var motorcycle = new Motorcycle(
            id: faker.Random.Guid().ToString(),
            licensePlate: new LicensePlate(faker.Random.String2(8, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")),
            year: faker.Random.Int(MotorcycleRules.MinYear, DateTime.Now.Year),
            model: faker.Vehicle.Model());

        var deliveryPerson = new DeliveryPerson(
            id: faker.Random.Guid().ToString(),
            name: faker.Person.FullName,
            cnpj: new Cnpj(faker.Company.Cnpj()),
            dateOfBirth: faker.Date.Past(30),
            cnh: new Cnh(faker.Random.String2(11, "0123456789"), CnhType.A),
            cnhImageUrl: faker.Internet.Url());

        var rental = new Rental(
            motorcycleId: motorcycle.Id,
            deliveryPersonId: deliveryPerson.Id,
            planType: RentalPlanType.SevenDays,
            startDate: DateTimeOffset.Now.Date.AddDays(1),
            endDate: DateTimeOffset.Now.Date.AddDays(7),
            expectedEndDate: DateTimeOffset.Now.Date.AddDays(7),
            timeProvider: TimeProvider.System);

        DbContext.Motorcycles.Add(motorcycle);
        DbContext.DeliveryPersons.Add(deliveryPerson);
        DbContext.Rentals.Add(rental);
        await DbContext.SaveChangesAsync();

        return rental;
    }
}