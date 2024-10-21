using FluentAssertions;
using MediatR;
using Mfm.Api.Controllers.V1;
using Mfm.Application.Dtos.DeliveryPersons;
using Mfm.Application.UseCases.DeliveryPersons.CreateDeliveryPerson;
using Mfm.Application.UseCases.DeliveryPersons.UpdateDeliveryPersonCnhImage;
using Mfm.Domain.Entities.Enums;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Mfm.Api.UnitTests.Controllers;
public sealed class DeliveryPersonsControllerTests
{
    private readonly IMediator _mediator;
    private readonly ILogger<DeliveryPersonsController> _logger;
    private readonly DeliveryPersonsController _controller;

    public DeliveryPersonsControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _logger = Substitute.For<ILogger<DeliveryPersonsController>>();
        _controller = new DeliveryPersonsController(_logger, _mediator);
    }

    [Fact]
    public async Task CreateDeliveryPerson_ShouldReturnCreatedAtRoute_WhenDeliveryPersonIsCreatedSuccessfully()
    {
        // Arrange
        var cnhImageBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/wcAAwAB/Kh0fQAAAABJRU5ErkJggg==";
        var deliveryPerson = new DeliveryPersonDto
        {
            Id = "deliveryperson-id",
            Name = "John Doe",
            Cnpj = "12345678000195",
            DateOfBirth = new DateTime(1990, 5, 20),
            CnhNumber = "12345678910",
            CnhType = CnhType.A,
            CnhImage = cnhImageBase64,
        };

        var input = new CreateDeliveryPersonInput(deliveryPerson);
        var output = new CreateDeliveryPersonOutput();

        _mediator.Send(Arg.Any<CreateDeliveryPersonInput>(), Arg.Any<CancellationToken>())
            .Returns(output);

        var cancellationToken = new CancellationToken();

        // Act
        var result = await _controller.CreateDeliveryPerson(deliveryPerson, cancellationToken);

        // Assert
        var createdAtRouteResult = result as ObjectResult;
        createdAtRouteResult.Should().NotBeNull();
        createdAtRouteResult!.StatusCode.Should().Be(StatusCodes.Status201Created);

        await _mediator.Received(1).Send(Arg.Is<CreateDeliveryPersonInput>(input =>
            input.DeliveryPerson.Id == "deliveryperson-id" &&
            input.DeliveryPerson.Cnpj == "12345678000195" &&
            input.DeliveryPerson.CnhNumber == "12345678910" &&
            input.DeliveryPerson.CnhType == CnhType.A &&
            input.DeliveryPerson.Name == "John Doe" &&
            input.DeliveryPerson.CnhImage == cnhImageBase64),
            cancellationToken);
    }

    [Fact]
    public async Task UpdateDeliveryPersonCnhImage_ShouldReturnOk_WhenCnhIsUpdatedSuccessfully()
    {
        // Arrange
        var deliveryPersonId = Guid.NewGuid().ToString();
        var cnhImageBase64 = "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mP8/wcAAwAB/Kh0fQAAAABJRU5ErkJggg==";
        var updateCnhImage = new UpdateCnhImageDto
        {
            CnhImage = cnhImageBase64,
        };

        var input = new UpdateDeliveryPersonCnhImageInput(deliveryPersonId, updateCnhImage.CnhImage);
        var output = new UpdateDeliveryPersonCnhImageOutput();

        _mediator.Send(Arg.Any<UpdateDeliveryPersonCnhImageInput>(), Arg.Any<CancellationToken>())
            .Returns(output);

        var cancellationToken = new CancellationToken();

        // Act
        var result = await _controller.UpdateDeliveryPersonCnhImage(
            deliveryPersonId,
            updateCnhImage,
            cancellationToken);

        // Assert
        var createdAtRouteResult = result as ObjectResult;
        createdAtRouteResult.Should().NotBeNull();
        createdAtRouteResult!.StatusCode.Should().Be(StatusCodes.Status201Created);

        await _mediator.Received(1).Send(
            Arg.Is<UpdateDeliveryPersonCnhImageInput>(x => x.Id == input.Id && x.CnhImage == input.CnhImage),
            cancellationToken);
    }
}
