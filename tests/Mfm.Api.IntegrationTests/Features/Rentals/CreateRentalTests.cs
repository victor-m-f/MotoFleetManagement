using Bogus;
using FluentAssertions;
using Mfm.Api.IntegrationTests.Features.Base;
using Mfm.Api.IntegrationTests.Support;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.Enums;
using Mfm.Domain.Entities.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Mfm.Api.IntegrationTests.Features.Rentals;
public class CreateRentalTests : FeatureTestsBase
{
    public CreateRentalTests(ApiFactory apiFactory)
        : base(apiFactory)
    {
    }

    [Fact]
    public async Task ShouldCreateRentalAndReturnCreatedAtRoute()
    {
        // Arrange
        await SeedAsync();
        var rental = new
        {
            entregador_id = "seed-deliveryperson-id",
            moto_id = "seed-motorcycle-id",
            data_inicio = DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            data_termino = DateTime.Now.Date.AddDays(7).AddSeconds(-1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            data_previsao_termino = DateTime.Now.Date.AddDays(7).AddSeconds(-1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            plano = 7
        };
        var rentalContent = new StringContent(
            JsonConvert.SerializeObject(rental),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await HttpClient.PostAsync("/locacao", rentalContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdRental = await DbContext.Rentals.FirstOrDefaultAsync();
        createdRental.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenDeliveryPersonDoesNotExist()
    {
        // Arrange
        await SeedAsync();
        var faker = new Faker();

        var rental = new
        {
            identificador = faker.Random.Guid().ToString(),
            entregador_id = "non-existent",
            moto_id = "seed-motorcycle-id",
            data_inicio = DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            data_termino = DateTime.Now.Date.AddDays(7).AddSeconds(-1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            data_previsao_termino = DateTime.Now.Date.AddDays(7).AddSeconds(-1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            plano = 7
        };
        var rentalContent = new StringContent(JsonConvert.SerializeObject(rental), Encoding.UTF8, "application/json");

        // Act
        var response = await HttpClient.PostAsync("/locacao", rentalContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain($"DeliveryPerson with Id '{rental.entregador_id}' was not found.");
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenMotorcycleDoesNotExist()
    {
        // Arrange
        await SeedAsync();
        var faker = new Faker();

        var rental = new
        {
            identificador = faker.Random.Guid().ToString(),
            entregador_id = "seed-deliveryperson-id",
            moto_id = "non-existent",
            data_inicio = DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            data_termino = DateTime.Now.Date.AddDays(7).AddSeconds(-1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            data_previsao_termino = DateTime.Now.Date.AddDays(7).AddSeconds(-1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            plano = 7
        };
        var rentalContent = new StringContent(JsonConvert.SerializeObject(rental), Encoding.UTF8, "application/json");

        // Act
        var response = await HttpClient.PostAsync("/locacao", rentalContent);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain($"Motorcycle with Id '{rental.moto_id}' was not found.");
    }

    [Fact]
    public async Task ShouldReturnConflict_WhenMotorcycleIsUnavailable()
    {
        // Arrange
        var faker = new Faker();
        await SeedAsync();

        var rental1 = new
        {
            identificador = faker.Random.Guid().ToString(),
            entregador_id = "seed-deliveryperson-id",
            moto_id = "seed-motorcycle-id",
            data_inicio = DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            data_termino = DateTime.Now.Date.AddDays(7).AddSeconds(-1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            data_previsao_termino = DateTime.Now.Date.AddDays(7).AddSeconds(-1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            plano = 7
        };
        var rentalContent1 = new StringContent(
            JsonConvert.SerializeObject(rental1),
            Encoding.UTF8,
            "application/json");
        var rentalResponse1 = await HttpClient.PostAsync("/locacao", rentalContent1);
        rentalResponse1.StatusCode.Should().Be(HttpStatusCode.Created);

        var rental2 = new
        {
            identificador = faker.Random.Guid().ToString(),
            entregador_id = "seed-deliveryperson-id",
            moto_id = "seed-motorcycle-id",
            data_inicio = DateTime.Now.Date.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            data_termino = DateTime.Now.Date.AddDays(10).AddSeconds(-1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            data_previsao_termino = DateTime.Now.Date.AddDays(10).AddSeconds(-1).ToString("yyyy-MM-ddTHH:mm:ssZ"),
            plano = 7
        };
        var rentalContent2 = new StringContent(
            JsonConvert.SerializeObject(rental2),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await HttpClient.PostAsync("/locacao", rentalContent2);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Conflict);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain("The motorcycle is not available during the requested period.");
    }

    [Theory]
    [InlineData(null, "motorcycle-id", "2024-01-01T00:00:00Z", "2024-01-07T23:59:59Z", "2024-01-07T23:59:59Z", 7, "The DeliveryPersonId field is required.")]
    [InlineData("deliveryperson-id", null, "2024-01-01T00:00:00Z", "2024-01-07T23:59:59Z", "2024-01-07T23:59:59Z", 7, "The MotorcycleId field is required.")]
    [InlineData("deliveryperson-id", "motorcycle-id", null, "2024-01-07T23:59:59Z", "2024-01-07T23:59:59Z", 7, "The StartDate field is required.")]
    [InlineData("deliveryperson-id", "motorcycle-id", "2024-01-01T00:00:00Z", null, "2024-01-07T23:59:59Z", 7, "The EndDate field is required.")]
    [InlineData("deliveryperson-id", "motorcycle-id", "2024-01-01T00:00:00Z", "2024-01-07T23:59:59Z", null, 7, "The ExpectedEndDate field is required.")]
    public async Task ShouldReturnBadRequest_WhenValidationFails(
        string deliveryPersonId,
        string motorcycleId,
        string startDate,
        string endDate,
        string expectedEndDate,
        int? plan,
        string errorMessage)
    {
        // Arrange
        var rental = new
        {
            entregador_id = deliveryPersonId,
            moto_id = motorcycleId,
            data_inicio = startDate,
            data_termino = endDate,
            data_previsao_termino = expectedEndDate,
            plano = plan
        };
        var content = new StringContent(JsonConvert.SerializeObject(rental), Encoding.UTF8, "application/json");

        // Act
        var response = await HttpClient.PostAsync("/locacao", content);

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
        if (!DbContext.DeliveryPersons.Any())
        {
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
    }

    private async Task SeedMotorcyclesAsync()
    {
        if (!DbContext.Motorcycles.Any())
        {
            var motorcycle = new Motorcycle(
                id: "seed-motorcycle-id",
                licensePlate: new LicensePlate("SEED1234"),
                year: DateTime.Now.Year,
                model: "Seed Model");

            DbContext.Motorcycles.Add(motorcycle);
            await DbContext.SaveChangesAsync();
        }
    }
}