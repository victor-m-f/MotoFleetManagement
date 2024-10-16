using Bogus;
using FluentAssertions;
using Mfm.Api.IntegrationTests.Support;
using Mfm.Domain.Entities.Rules;
using Newtonsoft.Json;
using System.Text;

namespace Mfm.Api.IntegrationTests.Features;
public class CreateMotorcycleTests : FeatureTestsBase
{
    public CreateMotorcycleTests(ApiFactory apiFactory)
        : base(apiFactory)
    {
    }

    [Fact]
    public async Task ShouldCreateMotorcycleAndReturnCreatedAtRoute()
    {
        // Arrange
        var faker = new Faker();
        var motorcycle = new
        {
            identificador = faker.Random.Guid().ToString(),
            placa = faker.Random.String(MotorcycleRules.LicensePlateMaxLength),
            ano = faker.Random.Int(MotorcycleRules.MinYear, DateTime.Now.Year),
            modelo = faker.Vehicle.Model()
        };

        var content = new StringContent(JsonConvert.SerializeObject(motorcycle), Encoding.UTF8, "application/json");

        // Act
        var response = await HttpClient.PostAsync("/motos", content);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        var createdMotorcycle = await DbContext.Motorcycles.FindAsync(motorcycle.identificador);
        createdMotorcycle.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldCreateMotorcycle2024_WhenYearIs2024()
    {
        // Arrange
        var faker = new Faker();
        var motorcycle = new
        {
            identificador = faker.Random.Guid().ToString(),
            placa = faker.Random.String(MotorcycleRules.LicensePlateMaxLength),
            ano = 2024,
            modelo = faker.Vehicle.Model()
        };

        var content = new StringContent(JsonConvert.SerializeObject(motorcycle), Encoding.UTF8, "application/json");

        // Act
        var response = await HttpClient.PostAsync("/motos", content);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        var createdMotorcycle = await DbContext.Motorcycles.FindAsync(motorcycle.identificador);
        createdMotorcycle.Should().NotBeNull();
        var createdMotorcycle2024 = await DbContext.Motorcycles2024.FindAsync(motorcycle.identificador);
        createdMotorcycle2024.Should().NotBeNull();
    }

    [Theory]
    [InlineData(null, "ABC12345", 2024, "ModelX", "The Id field is required.")]
    [InlineData("123", "", 2024, "ModelX", "The field LicensePlate must be a string with a minimum length of 8 and a maximum length of 8.")]
    [InlineData("123", "ABC12345", 1800, "ModelX", "The field Year must be between 1900 and 2147483647.")]
    [InlineData("123", "ABC12345", 2024, "", "The Model field is required.")]
    public async Task ShouldReturnBadRequest_WhenValidationFails(
        string id, string licensePlate, int year, string model, string errorMessage)
    {
        // Arrange
        var motorcycle = new
        {
            identificador = id,
            placa = licensePlate,
            ano = year,
            modelo = model,
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(motorcycle),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await HttpClient.PostAsync("/motos", content);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain(errorMessage);
    }
}
