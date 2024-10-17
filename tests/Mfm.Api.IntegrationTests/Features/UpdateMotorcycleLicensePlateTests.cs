using FluentAssertions;
using Mfm.Api.IntegrationTests.Features.Base;
using Mfm.Api.IntegrationTests.Support;
using Mfm.Application.UseCases.Motorcycles.UpdateMotorcycleLicensePlate;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.ValueObjects;
using Newtonsoft.Json;
using System.Text;

namespace Mfm.Api.IntegrationTests.Features;
public class UpdateMotorcycleLicensePlateTests : FeatureTestsBase
{
    public UpdateMotorcycleLicensePlateTests(ApiFactory apiFactory)
        : base(apiFactory)
    {
    }

    [Fact]
    public async Task ShouldUpdateMotorcycleLicensePlate_WhenDataIsValid()
    {
        // Arrange
        var motorcycle = await SeedMotorcycleAsync("1", "ABC12345");

        var updateRequest = new
        {
            placa = "NEW12345",
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(updateRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await HttpClient.PutAsync($"/motos/{motorcycle.Id}/placa", content);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);

        DbContext.ChangeTracker.Clear();

        var updatedMotorcycle = await DbContext.Motorcycles.FindAsync(motorcycle.Id);
        updatedMotorcycle!.LicensePlate.Value.Should().Be("NEW12345");
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenLicensePlateAlreadyExists()
    {
        // Arrange
        await SeedMotorcycleAsync("1", "ABC12345");
        await SeedMotorcycleAsync("2", "NEW12345");

        var updateRequest = new
        {
            placa = "NEW12345",
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(updateRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await HttpClient.PutAsync("/motos/1/placa", content);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain(UpdateMotorcycleLicensePlateOutput.SameLicensePlateErrorMessage);
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenMotorcycleDoesNotExist()
    {
        // Arrange
        var nonExistentId = "NON_EXISTENT_ID";
        var updateRequest = new
        {
            placa = "NEW12345",
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(updateRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await HttpClient.PutAsync($"/motos/{nonExistentId}/placa", content);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        var responseContent = await response.Content.ReadAsStringAsync();
        var expectedOutput = UpdateMotorcycleLicensePlateOutput.CreateNotFoundError(nonExistentId);
        responseContent.Should()
            .Contain(
            string.Format(
                expectedOutput.Errors.First(), nonExistentId));
    }

    [Theory]
    [InlineData("", "The field LicensePlate must be a string with a minimum length of 8 and a maximum length of 8.")]
    [InlineData("ABC", "The field LicensePlate must be a string with a minimum length of 8 and a maximum length of 8.")]
    [InlineData("ABC-123456", "The field LicensePlate must be a string with a minimum length of 8 and a maximum length of 8.")]
    public async Task ShouldReturnBadRequest_WhenValidationFails(string licensePlate, string errorMessage)
    {
        // Arrange
        var updateRequest = new
        {
            placa = licensePlate,
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(updateRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await HttpClient.PutAsync("/motos/any/placa", content);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain(errorMessage);
    }

    private async Task<Motorcycle> SeedMotorcycleAsync(string id, string licensePlate)
    {
        var motorcycle = new Motorcycle(id, new LicensePlate(licensePlate), 2024, "ModelX");
        DbContext.Motorcycles.Add(motorcycle);
        await DbContext.SaveChangesAsync();
        return motorcycle;
    }
}
