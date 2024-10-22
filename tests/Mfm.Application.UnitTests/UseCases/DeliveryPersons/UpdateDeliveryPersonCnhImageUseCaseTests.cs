using FluentAssertions;
using Mfm.Application.UseCases.DeliveryPersons.UpdateDeliveryPersonCnhImage;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.Enums;
using Mfm.Domain.Entities.ValueObjects;
using Mfm.Domain.Repositories;
using Mfm.Domain.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Net;

namespace Mfm.Application.UnitTests.UseCases.DeliveryPersons;
public sealed class UpdateDeliveryPersonCnhImageUseCaseTests
{
    [Fact]
    public async Task Handle_ShouldUpdateDeliveryPersonCnhImage_WhenCalledWithValidData()
    {
        // Arrange
        var deliveryPersonId = Guid.NewGuid().ToString();
        var deliveryPerson = new DeliveryPerson(
            deliveryPersonId,
            "Name",
            new Cnpj("12345678000195"),
            new DateTime(1991, 2, 13),
            new Cnh("12345678910", CnhType.A),
            $"{deliveryPersonId}.png");
        var repository = Substitute.For<IDeliveryPersonRepository>();
        var storageService = Substitute.For<IStorageService>();

        var request = new UpdateDeliveryPersonCnhImageInput(
            deliveryPersonId,
            "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/wcAAwAB/Kh0fQAAAABJRU5ErkJggg==");

        repository.GetByIdAsync(request.Id, Arg.Any<CancellationToken>()).Returns(deliveryPerson);

        var useCase = new UpdateDeliveryPersonCnhImageUseCase(
            Substitute.For<ILogger<UpdateDeliveryPersonCnhImageUseCase>>(),
            repository,
            storageService);

        var cancellationToken = new CancellationToken();

        // Act
        var result = await useCase.Handle(request, cancellationToken);

        // Assert
        await repository
            .Received(1)
            .GetByIdAsync(request.Id, cancellationToken);

        await storageService
            .Received(1)
            .CreateBlobFileAsync(
            $"{request.Id}.png",
            Arg.Any<byte[]>(),
            cancellationToken);

        result.Should().BeOfType<UpdateDeliveryPersonCnhImageOutput>();
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenDeliveryPersonDoesntExists()
    {
        // Arrange
        var repository = Substitute.For<IDeliveryPersonRepository>();
        var storageService = Substitute.For<IStorageService>();

        var request = new UpdateDeliveryPersonCnhImageInput(
            "non-existent-id",
            "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/wcAAwAB/Kh0fQAAAABJRU5ErkJggg==");

        var useCase = new UpdateDeliveryPersonCnhImageUseCase(
            Substitute.For<ILogger<UpdateDeliveryPersonCnhImageUseCase>>(),
            repository,
            storageService);

        var cancellationToken = new CancellationToken();

        // Act
        var result = await useCase.Handle(request, cancellationToken);

        // Assert
        await repository
            .Received(1)
            .GetByIdAsync(request.Id, cancellationToken);

        await storageService
            .Received(0)
            .CreateBlobFileAsync(
            Arg.Any<string>(),
            Arg.Any<byte[]>(),
            cancellationToken);

        result.Should().BeOfType<UpdateDeliveryPersonCnhImageOutput>();
        result.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
