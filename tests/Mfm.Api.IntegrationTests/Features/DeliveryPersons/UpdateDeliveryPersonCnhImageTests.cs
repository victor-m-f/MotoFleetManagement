using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Mfm.Api.IntegrationTests.Features.Base;
using Mfm.Api.IntegrationTests.Support;
using Mfm.Domain.Entities.Enums;
using Newtonsoft.Json;
using System.Text;

namespace Mfm.Api.IntegrationTests.Features.DeliveryPersons;
public class UpdateDeliveryPersonCnhImageTests : FeatureTestsBase
{
    private readonly string _validBase64;

    public UpdateDeliveryPersonCnhImageTests(ApiFactory apiFactory)
        : base(apiFactory)
    {
        _validBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/wcAAwAB/Kh0fQAAAABJRU5ErkJggg==";
    }

    [Fact]
    public async Task ShouldUpdateCnhImageAndReturnOk()
    {
        // Arrange
        var faker = new Faker();
        var deliveryPersonId = faker.Random.Guid().ToString();
        var deliveryPerson = new
        {
            identificador = deliveryPersonId,
            nome = faker.Person.FullName,
            cnpj = faker.Company.Cnpj(),
            data_nascimento = "1990-01-01T00:00:00Z",
            numero_cnh = faker.Random.String2(11, "0123456789"),
            tipo_cnh = CnhType.A.ToString(),
            imagem_cnh = _validBase64
        };

        var content = new StringContent(JsonConvert.SerializeObject(deliveryPerson), Encoding.UTF8, "application/json");
        await HttpClient.PostAsync("/entregadores", content);

        var updateRequest = new
        {
            imagem_cnh = _validBase64
        };

        var updateContent = new StringContent(JsonConvert.SerializeObject(updateRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await HttpClient.PostAsync($"/entregadores/{deliveryPersonId}/cnh", updateContent);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        var cnhImageUploaded = await StorageService.GetBlobFileAsync($"{deliveryPersonId}.png", CancellationToken.None);
        cnhImageUploaded.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldReturnNotFound_WhenDeliveryPersonDoesNotExist()
    {
        // Arrange
        var invalidDeliveryPersonId = "invalid-id";
        var updateRequest = new
        {
            imagem_cnh = _validBase64
        };

        var content = new StringContent(
            JsonConvert.SerializeObject(updateRequest),
            Encoding.UTF8,
            "application/json");

        // Act
        var response = await HttpClient.PostAsync($"/entregadores/{invalidDeliveryPersonId}/cnh", content);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should()
            .Contain($"DeliveryPerson with Id '{invalidDeliveryPersonId}' was not found.");
    }

    [Theory]
    [InlineData("data:image/jpg;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/wcAAwAB/Kh0fQAAAABJRU5ErkJggg==", "The image must be in one of the following formats: png, bmp.")]
    [InlineData("", "The CnhImage field is required.")]
    public async Task ShouldReturnBadRequest_WhenValidationFails(string base64Image, string expectedMessage)
    {
        // Arrange
        var updateRequest = new
        {
            imagem_cnh = base64Image
        };

        var content = new StringContent(JsonConvert.SerializeObject(updateRequest), Encoding.UTF8, "application/json");

        // Act
        var response = await HttpClient.PostAsync("/entregadores/valid-id/cnh", content);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain(expectedMessage);
    }
}