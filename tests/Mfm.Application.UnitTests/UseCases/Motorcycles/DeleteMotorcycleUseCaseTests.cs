using FluentAssertions;
using Mfm.Application.UseCases.Motorcycles.DeleteMotorcycle;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.Enums;
using Mfm.Domain.Entities.ValueObjects;
using Mfm.Domain.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Net;

namespace Mfm.Application.UnitTests.UseCases.Motorcycles;
public sealed class DeleteMotorcycleUseCaseTests
{
    [Fact]
    public async Task Handle_ShouldDeleteMotorcycle_WhenNoRentalsExist()
    {
        // Arrange
        var motorcycleRepository = Substitute.For<IMotorcycleRepository>();
        var logger = Substitute.For<ILogger<DeleteMotorcycleUseCase>>();

        var motorcycleId = "motorcycle-id";
        var motorcycle = new Motorcycle(
            id: motorcycleId,
            licensePlate: new LicensePlate("ABC-1234"),
            year: 2022,
            model: "Test Model");

        motorcycleRepository.GetByIdAsync(motorcycleId, true, CancellationToken.None)
            .Returns(motorcycle);

        var useCase = new DeleteMotorcycleUseCase(logger, motorcycleRepository);

        var input = new DeleteMotorcycleInput(motorcycleId);

        // Act
        var result = await useCase.Handle(input, CancellationToken.None);

        // Assert
        result.IsValid.Should().BeTrue();
        motorcycleRepository.Received(1).Remove(motorcycle);
        await motorcycleRepository.Received(1).SaveChangesAsync(CancellationToken.None);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenMotorcycleHasRentals()
    {
        // Arrange
        var motorcycleRepository = Substitute.For<IMotorcycleRepository>();
        var logger = Substitute.For<ILogger<DeleteMotorcycleUseCase>>();

        var motorcycleId = "motorcycle-id";
        var motorcycle = new Motorcycle(
            id: motorcycleId,
            licensePlate: new LicensePlate("ABC-1234"),
            year: 2022,
            model: "Test Model");

        var rental = new Rental(
            motorcycleId,
            "deliveryPersonId",
            RentalPlanType.SevenDays,
            DateTimeOffset.Now.AddDays(1),
            DateTimeOffset.Now.AddDays(7),
            DateTimeOffset.Now.AddDays(7),
            TimeProvider.System);

        motorcycle.Rentals.Add(rental);

        motorcycleRepository.GetByIdAsync(motorcycleId, true, CancellationToken.None)
            .Returns(motorcycle);

        var useCase = new DeleteMotorcycleUseCase(logger, motorcycleRepository);

        var input = new DeleteMotorcycleInput(motorcycleId);

        // Act
        var result = await useCase.Handle(input, CancellationToken.None);

        // Assert
        result.IsValid.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.Conflict);
        motorcycleRepository.DidNotReceive().Remove(Arg.Any<Motorcycle>());
        await motorcycleRepository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
