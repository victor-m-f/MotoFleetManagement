using FluentAssertions;
using Mfm.Api.IntegrationTests.Features.Base;
using Mfm.Api.IntegrationTests.Support;
using Mfm.Application.Dtos.Motorcycles;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.ValueObjects;
using Newtonsoft.Json;

namespace Mfm.Api.IntegrationTests.Features.Motorcycles;
public class GetMotorcyclesTests : FeatureTestsBase
{
    public GetMotorcyclesTests(ApiFactory apiFactory)
        : base(apiFactory)
    {
    }

    [Fact]
    public async Task ShouldReturnAllMotorcycles_WhenNoFilterIsApplied()
    {
        // Arrange
        await SeedMotorcyclesAsync();

        // Act
        var response = await HttpClient.GetAsync("/motos");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        var motorcycles = JsonConvert.DeserializeObject<List<MotorcycleDto>>(responseContent);

        motorcycles.Should().NotBeNull();
        motorcycles.Should().HaveCount(2);
    }

    [Fact]
    public async Task ShouldReturnFilteredMotorcycles_WhenLicensePlateIsProvided()
    {
        // Arrange
        await SeedMotorcyclesAsync();
        var licensePlate = "ABC12345";

        // Act
        var response = await HttpClient.GetAsync($"/motos?licensePlate={licensePlate}");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        var motorcycles = JsonHelper.Deserialize<List<MotorcycleDto>>(responseContent);

        motorcycles.Should().NotBeNull();
        motorcycles.Should().HaveCount(1);
        motorcycles!.First().LicensePlate.Should().Be(licensePlate);
    }

    [Fact]
    public async Task ShouldReturnEmptyList_WhenLicensePlateDoesNotExist()
    {
        // Arrange
        await SeedMotorcyclesAsync();

        // Act
        var response = await HttpClient.GetAsync("/motos?licensePlate=NOTFOUND");

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);

        var responseContent = await response.Content.ReadAsStringAsync();
        var motorcycles = JsonConvert.DeserializeObject<List<MotorcycleDto>>(responseContent);

        motorcycles.Should().NotBeNull();
        motorcycles.Should().BeEmpty();
    }

    private async Task SeedMotorcyclesAsync()
    {
        var motorcycles = new List<Motorcycle>
        {
            new("1", new LicensePlate("ABC12345"), 2024, "ModelX"),
            new("2", new LicensePlate("XYZ67890"), 2023, "ModelY")
        };

        DbContext.Motorcycles.AddRange(motorcycles);
        await DbContext.SaveChangesAsync();
    }
}
