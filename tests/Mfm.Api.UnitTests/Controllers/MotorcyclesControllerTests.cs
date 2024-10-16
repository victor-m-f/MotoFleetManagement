using FluentAssertions;
using MediatR;
using Mfm.Api.Controllers.V1;
using Mfm.Application.Dtos.Motorcycles;
using Mfm.Application.UseCases.Motorcycles.CreateMotorcycle;
using Mfm.Application.UseCases.Motorcycles.UpdateMotorcycleLicensePlate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Mfm.Api.UnitTests.Controllers;
public sealed class MotorcyclesControllerTests
{
    private readonly IMediator _mediator;
    private readonly ILogger<MotorcyclesController> _logger;
    private readonly MotorcyclesController _controller;

    public MotorcyclesControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _logger = Substitute.For<ILogger<MotorcyclesController>>();
        _controller = new MotorcyclesController(_logger, _mediator);
    }

    [Fact]
    public async Task CreateMotorcycle_ShouldReturnCreatedAtRoute_WhenMotorcycleIsCreatedSuccessfully()
    {
        // Arrange
        var motorcycle = new MotorcycleDto
        {
            Id = "motorcycle-id",
            LicensePlate = "ABC-1234",
            Year = 2024,
            Model = "Test Model"
        };

        var input = new CreateMotorcycleInput(motorcycle);
        var output = new CreateMotorcycleOutput();

        _mediator.Send(Arg.Any<CreateMotorcycleInput>(), Arg.Any<CancellationToken>())
            .Returns(output);

        var cancellationToken = new CancellationToken();

        // Act
        var result = await _controller.CreateMotorcycle(motorcycle, cancellationToken);

        // Assert
        var createdAtRouteResult = result as CreatedAtRouteResult;
        createdAtRouteResult.Should().NotBeNull();
        createdAtRouteResult!.RouteName.Should().Be(nameof(_controller.GetMotorcycleById));
        createdAtRouteResult.RouteValues.Should().ContainKey("id");
        createdAtRouteResult.RouteValues!["id"].Should().Be(motorcycle.Id);
        createdAtRouteResult.Value.Should().Be(motorcycle);

        await _mediator.Received(1).Send(Arg.Is<CreateMotorcycleInput>(input =>
            input.Motorcycle.Id == "motorcycle-id" &&
            input.Motorcycle.LicensePlate == "ABC-1234" &&
            input.Motorcycle.Year == 2024 &&
            input.Motorcycle.Model == "Test Model"), cancellationToken);
        _logger.Received(1).Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("Response")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task UpdateLicensePlate_ShouldReturnNoContent_WhenLicensePlateIsUpdated()
    {
        // Arrange
        var mediator = Substitute.For<IMediator>();
        mediator.Send(Arg.Any<UpdateMotorcycleLicensePlateInput>(), Arg.Any<CancellationToken>())
            .Returns(new UpdateMotorcycleLicensePlateOutput());

        var controller = new MotorcyclesController(Substitute.For<ILogger<MotorcyclesController>>(), mediator);

        // Act
        var result = await controller
            .UpdateMotorcycleLicensePlate(
            "1",
            new UpdateLicensePlateDto { LicensePlate = "XYZ-5678", },
            CancellationToken.None);

        // Assert
        var objectResult = (result as ObjectResult)!;
        objectResult.StatusCode.Should().Be(StatusCodes.Status204NoContent);
    }
}
