using FluentAssertions;
using MediatR;
using Mfm.Api.Controllers.V1;
using Mfm.Application.Dtos.Motorcycles;
using Mfm.Application.UseCases.Motorcycles.CreateMotorcycle;
using Mfm.Application.UseCases.Motorcycles.DeleteMotorcycle;
using Mfm.Application.UseCases.Motorcycles.GetMotorcycleById;
using Mfm.Application.UseCases.Motorcycles.GetMotorcycles;
using Mfm.Application.UseCases.Motorcycles.UpdateMotorcycleLicensePlate;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using static MassTransit.ValidationResultExtensions;

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
        var objectResult = (result as OkResult)!;
        objectResult.StatusCode.Should().Be(StatusCodes.Status200OK);
    }

    [Fact]
    public async Task GetMotorcycles_ShouldReturnOk_WithListOfMotorcycles()
    {
        // Arrange
        var motorcycles = new List<MotorcycleDto>
        {
            new() { Id = "1", LicensePlate = "ABC-1234", Year = 2024, Model = "Model X" },
            new() { Id = "2", LicensePlate = "XYZ-5678", Year = 2023, Model = "Model Y" }
        };

        var output = new GetMotorcyclesOutput(motorcycles);

        _mediator.Send(Arg.Any<GetMotorcyclesInput>(), Arg.Any<CancellationToken>())
            .Returns(output);

        var cancellationToken = new CancellationToken();

        // Act
        var result = await _controller.GetMotorcycles(null, cancellationToken);

        // Assert
        var okResult = result as ObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(motorcycles);

        await _mediator.Received(1).Send(Arg.Any<GetMotorcyclesInput>(), cancellationToken);
    }

    [Fact]
    public async Task GetMotorcycleById_ShouldReturnOk_WithMotorcycle()
    {
        // Arrange
        var motorcycle = new MotorcycleDto
        {
            Id = "1",
            LicensePlate = "ABC-1234",
            Year = 2024,
            Model = "Model X"
        };

        var output = new GetMotorcycleByIdOutput(motorcycle);

        _mediator.Send(Arg.Any<GetMotorcycleByIdInput>(), Arg.Any<CancellationToken>())
            .Returns(output);

        var cancellationToken = new CancellationToken();

        // Act
        var result = await _controller.GetMotorcycleById("1", cancellationToken);

        // Assert
        var okResult = result as ObjectResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);
        okResult.Value.Should().BeEquivalentTo(motorcycle);

        await _mediator.Received(1).Send(Arg.Any<GetMotorcycleByIdInput>(), cancellationToken);
    }

    [Fact]
    public async Task DeleteMotorcycle_ShouldReturnOk_WhenDeletionIsSuccessful()
    {
        // Arrange
        var motorcycleId = "motorcycle-id";
        var cancellationToken = new CancellationToken();

        var output = new DeleteMotorcycleOutput();

        _mediator.Send(Arg.Any<DeleteMotorcycleInput>(), Arg.Any<CancellationToken>())
            .Returns(output);

        // Act
        var result = await _controller.DeleteMotorcycle(motorcycleId, cancellationToken);

        // Assert
        var okResult = result as OkResult;
        okResult.Should().NotBeNull();
        okResult!.StatusCode.Should().Be(StatusCodes.Status200OK);

        await _mediator.Received(1).Send(Arg.Is<DeleteMotorcycleInput>(input =>
            input.Id == motorcycleId), cancellationToken);
    }
}
