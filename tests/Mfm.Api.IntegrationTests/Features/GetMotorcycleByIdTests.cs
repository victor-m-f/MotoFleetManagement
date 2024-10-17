using FluentAssertions;
using Mfm.Api.IntegrationTests.Features.Base;
using Mfm.Api.IntegrationTests.Support;
using Mfm.Application.Dtos.Motorcycles;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.ValueObjects;

namespace Mfm.Api.IntegrationTests.Features;
public class GetMotorcycleByIdTests : FeatureTestsBase
{
    public GetMotorcycleByIdTests(ApiFactory apiFactory)
        : base(apiFactory)
    {
    }

    [Fact]
    public async Task ShouldReturnMotorcycle_WhenMotorcycleExists()
    {
        // Arrange
        var motorcycle = await SeedMotorcycleAsync("1", "ABC12345", 2024, "ModelX");

        // Act
        var response = await HttpClient.GetAsync($"/motos/{motorcycle.Id}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        var returnedMotorcycle = JsonHelper.Deserialize<MotorcycleDto>(responseContent);

        returnedMotorcycle.Should().NotBeNull();
        returnedMotorcycle!.Id.Should().Be(motorcycle.Id);
        returnedMotorcycle.LicensePlate.Should().Be(motorcycle.LicensePlate.Value);
        returnedMotorcycle.Year.Should().Be(motorcycle.Year);
        returnedMotorcycle.Model.Should().Be(motorcycle.Model);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenMotorcycleDoesNotExist()
    {
        // Act
        var response = await HttpClient.GetAsync("/motos/non-existing-id");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    private async Task<Motorcycle> SeedMotorcycleAsync(string id, string licensePlate, int year, string model)
    {
        var motorcycle = new Motorcycle(id, new LicensePlate(licensePlate), year, model);

        DbContext.Motorcycles.Add(motorcycle);
        await DbContext.SaveChangesAsync();

        return motorcycle;
    }
}
