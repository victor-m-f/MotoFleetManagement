using Bogus;
using Bogus.Extensions.Brazil;
using FluentAssertions;
using Mfm.Api.IntegrationTests.Features.Base;
using Mfm.Api.IntegrationTests.Support;
using Mfm.Application.UseCases.DeliveryPersons.CreateDeliveryPerson;
using Mfm.Domain.Entities.Enums;
using Mfm.Domain.Entities.Rules;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;

namespace Mfm.Api.IntegrationTests.Features.DeliveryPersons;
public class CreateDeliveryPersonTests : FeatureTestsBase
{
    private readonly string _validBase64;

    public CreateDeliveryPersonTests(ApiFactory apiFactory)
        : base(apiFactory)
    {
        _validBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/wcAAwAB/Kh0fQAAAABJRU5ErkJggg==";
    }

    [Fact]
    public async Task ShouldCreateDeliveryPersonAndReturnCreatedAtRoute()
    {
        // Arrange
        var faker = new Faker();
        var deliveryPerson = new
        {
            identificador = faker.Random.Guid().ToString(),
            nome = faker.Person.FullName,
            cnpj = faker.Company.Cnpj(),
            data_nascimento = "1990-01-01T00:00:00Z",
            numero_cnh = faker.Random.String2(DeliveryPersonRules.CnhNumberLength, "0123456789"),
            tipo_cnh = CnhType.A.ToString(),
            imagem_cnh = _validBase64,
        };

        var content = new StringContent(JsonConvert.SerializeObject(deliveryPerson), Encoding.UTF8, "application/json");

        // Act
        var response = await HttpClient.PostAsync("/entregadores", content);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        var createdDeliveryPerson = await DbContext.DeliveryPersons.FindAsync(deliveryPerson.identificador);
        createdDeliveryPerson.Should().NotBeNull();
        var cnhImageUploaded = await StorageService.GetBlobFileAsync(
            $"{deliveryPerson.identificador}.png",
            CancellationToken.None);

        cnhImageUploaded.Should().NotBeNull();
    }

    [Fact]
    public async Task ShouldReturnBadRequest_WhenCnpjIsDuplicated()
    {
        // Arrange
        var cnpj = "12345678000195";
        var faker = new Faker();
        var deliveryPerson = new
        {
            identificador = faker.Random.Guid().ToString(),
            nome = faker.Person.FullName,
            cnpj,
            data_nascimento = "1990-01-01T00:00:00Z",
            numero_cnh = faker.Random.String2(DeliveryPersonRules.CnhNumberLength, "0123456789"),
            tipo_cnh = CnhType.A.ToString(),
            imagem_cnh = _validBase64,
        };

        var content = new StringContent(JsonConvert.SerializeObject(deliveryPerson), Encoding.UTF8, "application/json");

        var firstResponse = await HttpClient.PostAsync("/entregadores", content);
        firstResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);

        // Act
        var secondResponse = await HttpClient.PostAsync("/entregadores", content);

        // Assert
        secondResponse.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        var responseContent = await secondResponse.Content.ReadAsStringAsync();
        responseContent.Should().Contain(CreateDeliveryPersonOutput.SameCnpjErrorMessage);

        var savedDeliveryPersons = await DbContext.DeliveryPersons.CountAsync();
        savedDeliveryPersons.Should().Be(1);
    }

    [Theory]
    [InlineData(null, "12345678000195", "12345678910", "A", "The Id field is required.")]
    [InlineData("123", "", "12345678910", "A", "The Cnpj field is required.")]
    [InlineData("123", "12345678000195", "", "A", "The CnhNumber field is required.")]
    [InlineData("123", "12345678000195", "12345678910", "", "tipo_cnh")]
    public async Task ShouldReturnBadRequest_WhenValidationFails(
        string id,
        string cnpj,
        string cnhNumber,
        string cnhType,
        string errorMessage)
    {
        // Arrange
        var deliveryPerson = new
        {
            identificador = id,
            nome = "John Doe",
            cnpj,
            data_nascimento = new DateTime(1990, 5, 20),
            numero_cnh = cnhNumber,
            tipo_cnh = cnhType,
            imagem_cnh = _validBase64,
        };

        var content = new StringContent(JsonConvert.SerializeObject(deliveryPerson), Encoding.UTF8, "application/json");

        // Act
        var response = await HttpClient.PostAsync("/entregadores", content);

        // Assert
        response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);

        var responseContent = await response.Content.ReadAsStringAsync();
        responseContent.Should().Contain(errorMessage);
    }
}