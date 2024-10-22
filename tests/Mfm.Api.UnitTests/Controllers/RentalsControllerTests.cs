﻿using FluentAssertions;
using MediatR;
using Mfm.Api.Controllers.V1;
using Mfm.Application.Dtos.Rentals;
using Mfm.Application.UseCases.Rentals.CreateRental;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Mfm.Api.UnitTests.Controllers;
public sealed class RentalsControllerTests
{
    private readonly IMediator _mediator;
    private readonly ILogger<RentalsController> _logger;
    private readonly RentalsController _controller;

    public RentalsControllerTests()
    {
        _mediator = Substitute.For<IMediator>();
        _logger = Substitute.For<ILogger<RentalsController>>();
        _controller = new RentalsController(_logger, _mediator);
    }

    [Fact]
    public async Task CreateRental_ShouldReturnCreatedAtRoute_WhenRentalIsCreatedSuccessfully()
    {
        // Arrange
        var startDate = DateTime.Now.Date.AddDays(1);
        var endDate = DateTime.Now.Date.AddDays(7);

        var rental = new CreateRentalDto
        {
            DeliveryPersonId = "delivery-person-id",
            MotorcycleId = "motorcycle-id",
            StartDate = startDate.ToString(),
            EndDate = endDate.ToString(),
            ExpectedEndDate = endDate.ToString(),
            Plan = 7,
        };

        var input = new CreateRentalInput(rental);
        var output = new CreateRentalOutput();

        _mediator.Send(Arg.Any<CreateRentalInput>(), Arg.Any<CancellationToken>())
            .Returns(output);

        var cancellationToken = new CancellationToken();

        // Act
        var result = await _controller.CreateRental(rental, cancellationToken);

        // Assert
        var createdAtRouteResult = result as ObjectResult;
        createdAtRouteResult.Should().NotBeNull();
        createdAtRouteResult!.StatusCode.Should().Be(StatusCodes.Status201Created);

        await _mediator.Received(1).Send(Arg.Is<CreateRentalInput>(input =>
            input.Rental.DeliveryPersonId == "delivery-person-id" &&
            input.Rental.MotorcycleId == "motorcycle-id" &&
            input.Rental.StartDate == startDate.ToString() &&
            input.Rental.EndDate == endDate.ToString() &&
            input.Rental.ExpectedEndDate == endDate.ToString() &&
            input.Rental.Plan == 7),
            cancellationToken);
    }
}
