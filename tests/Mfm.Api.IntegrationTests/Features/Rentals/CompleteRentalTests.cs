using FluentAssertions;
using Mfm.Api.IntegrationTests.Features.Base;
using Mfm.Api.IntegrationTests.Support;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.Enums;
using Mfm.Domain.Entities.ValueObjects;
using Mfm.Domain.Services;
using Microsoft.Extensions.Time.Testing;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Mfm.Api.IntegrationTests.Features.Rentals;
public class CompleteRentalTests : FeatureTestsBase
{
    public CompleteRentalTests(ApiFactory apiFactory)
        : base(apiFactory)
    {
    }

    [Fact]
    public async Task ShouldCompleteRentalWithEarlyReturnAndApplyFine()
    {
        // Arrange
        var initialTime = DateTimeOffset.UtcNow.Date;
        var timeProvider = new FakeTimeProvider(initialTime);

        await SeedAsync();

        var planType = RentalPlanType.SevenDays;
        var plan = RentalPlan.GetPlan(planType);

        var startDate = initialTime.AddDays(1);
        var expectedEndDate = startDate.AddDays(plan.DurationInDays - 1);
        var endDate = expectedEndDate;

        var rental = await SeedRentalAsync(
            deliveryPersonId: "seed-deliveryperson-id",
            motorcycleId: "seed-motorcycle-id",
            planType: planType,
            startDate: startDate,
            expectedEndDate: expectedEndDate,
            endDate: endDate);

        timeProvider.Advance(TimeSpan.FromDays(3));

        var returnDate = timeProvider.GetUtcNow().Date;

        var daysUsed = (returnDate - rental.Period.StartDate.Date).Days;
        var unusedDays = plan.DurationInDays - daysUsed;
        var baseCost = daysUsed * plan.DailyRate;
        var finePercentage = 0.20m;
        var fine = finePercentage * (unusedDays * plan.DailyRate);
        var expectedTotalCost = baseCost + fine;

        var completeRentalRequest = new
        {
            data_devolucao = returnDate.ToString("dd/MM/yyyy HH:mm:ss"),
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(completeRentalRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await HttpClient.PutAsync($"/locacao/{rental.Id}/devolucao", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        DbContext.ChangeTracker.Clear();
        var updatedRental = await DbContext.Rentals.FindAsync(rental.Id);
        updatedRental.Should().NotBeNull();
        updatedRental!.ReturnDate.Should().Be(returnDate);
        updatedRental.TotalCost.Should().Be(expectedTotalCost);
    }

    [Fact]
    public async Task ShouldCompleteRentalWithLateReturnAndApplyExtraCharges()
    {
        // Arrange
        var initialTime = DateTimeOffset.UtcNow.Date;
        var timeProvider = new FakeTimeProvider(initialTime);

        await SeedAsync();

        var planType = RentalPlanType.SevenDays;
        var plan = RentalPlan.GetPlan(planType);

        var startDate = initialTime.AddDays(1);
        var expectedEndDate = startDate.AddDays(plan.DurationInDays - 1);
        var endDate = expectedEndDate;

        var rental = await SeedRentalAsync(
            deliveryPersonId: "seed-deliveryperson-id",
            motorcycleId: "seed-motorcycle-id",
            planType: planType,
            startDate: startDate,
            expectedEndDate: expectedEndDate,
            endDate: endDate);

        timeProvider.Advance(TimeSpan.FromDays(9));
        var returnDate = timeProvider.GetUtcNow().Date;

        var actualDuration = (returnDate - rental.Period.StartDate.Date).Days;
        var expectedDuration = plan.DurationInDays;
        var extraDays = actualDuration - expectedDuration;
        var extraCharges = extraDays * 50m;
        var baseCost = plan.DailyRate * expectedDuration;
        var expectedTotalCost = baseCost + extraCharges;

        var completeRentalRequest = new
        {
            data_devolucao = returnDate.ToString("dd/MM/yyyy HH:mm:ss"),
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(completeRentalRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await HttpClient.PutAsync($"/locacao/{rental.Id}/devolucao", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        DbContext.ChangeTracker.Clear();
        var updatedRental = await DbContext.Rentals.FindAsync(rental.Id);
        updatedRental.Should().NotBeNull();
        updatedRental!.ReturnDate.Should().Be(returnDate);
        updatedRental.TotalCost.Should().Be(expectedTotalCost);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenRentalDoesNotExist()
    {
        // Arrange
        var initialTime = DateTimeOffset.UtcNow.Date;

        await SeedAsync();

        var nonExistentRentalId = "non-existent-id";
        var completeRentalRequest = new
        {
            data_devolucao = initialTime.ToString("dd/MM/yyyy HH:mm:ss")
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(completeRentalRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await HttpClient.PutAsync($"/locacao/{nonExistentRentalId}/devolucao", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain($"Rental with Id '{nonExistentRentalId}' was not found.");
    }

    [Theory]
    [InlineData(null, "The ReturnDate field is required.")]
    [InlineData("invalid-date", "ReturnDate is invalid.")]
    public async Task ShouldReturnBadRequest_WhenValidationFails(string returnDate, string errorMessage)
    {
        // Arrange
        var initialTime = DateTimeOffset.UtcNow.Date;

        await SeedAsync();

        var rental = await SeedRentalAsync(
            deliveryPersonId: "seed-deliveryperson-id",
            motorcycleId: "seed-motorcycle-id",
            planType: RentalPlanType.SevenDays,
            startDate: initialTime.AddDays(1),
            expectedEndDate: initialTime.AddDays(7),
            endDate: initialTime.AddDays(7));

        var completeRentalRequest = new
        {
            data_devolucao = returnDate
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(completeRentalRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await HttpClient.PutAsync($"/locacao/{rental.Id}/devolucao", content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain(errorMessage);
    }

    private async Task SeedAsync()
    {
        await SeedDeliveryPersonsAsync();
        await SeedMotorcyclesAsync();
    }

    private async Task SeedDeliveryPersonsAsync()
    {
        if (DbContext.DeliveryPersons.Any())
        {
            return;
        }

        var deliveryPerson = new DeliveryPerson(
                id: "seed-deliveryperson-id",
                name: "Seed Delivery Person",
                cnpj: new Cnpj("12345678000195"),
                dateOfBirth: new DateTime(1990, 1, 1),
                cnh: new Cnh("12345678910", CnhType.A),
                cnhImageUrl: "seed-image-url");

        DbContext.DeliveryPersons.Add(deliveryPerson);
        await DbContext.SaveChangesAsync();
    }

    private async Task SeedMotorcyclesAsync()
    {
        if (DbContext.Motorcycles.Any())
        {
            return;
        }

        var motorcycle = new Motorcycle(
                id: "seed-motorcycle-id",
                licensePlate: new LicensePlate("SEED1234"),
                year: DateTime.Now.Year,
                model: "Seed Model");

        DbContext.Motorcycles.Add(motorcycle);
        await DbContext.SaveChangesAsync();
    }

    private async Task<Rental> SeedRentalAsync(
        string deliveryPersonId,
        string motorcycleId,
        RentalPlanType planType,
        DateTimeOffset startDate,
        DateTimeOffset expectedEndDate,
        DateTimeOffset endDate)
    {
        var rental = new Rental(
            motorcycleId: motorcycleId,
            deliveryPersonId: deliveryPersonId,
            planType: planType,
            startDate: startDate,
            endDate: endDate,
            expectedEndDate: expectedEndDate,
            timeProvider: TimeProvider.System);

        DbContext.Rentals.Add(rental);
        await DbContext.SaveChangesAsync();
        return rental;
    }
}