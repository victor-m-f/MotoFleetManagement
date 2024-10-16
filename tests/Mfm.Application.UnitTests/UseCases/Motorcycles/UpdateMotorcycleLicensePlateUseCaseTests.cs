using FluentAssertions;
using Mfm.Application.UseCases.Motorcycles.UpdateMotorcycleLicensePlate;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.ValueObjects;
using Mfm.Domain.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ReceivedExtensions;

namespace Mfm.Application.UnitTests.UseCases.Motorcycles;
public sealed class UpdateMotorcycleLicensePlateUseCaseTests
{
    [Theory]
    [InlineData("ABC-1234", "XYZ-5678")]
    [InlineData("ABC-1234", "ABC-1234")]
    public async Task Handle_ShouldUpdateLicensePlate_WhenMotorcycleExists(
        string originalLicensePlate,
        string newLicensePlate)
    {
        // Arrange
        var motorcycle = new Motorcycle("1", new LicensePlate(originalLicensePlate), 2023, "ModelX");

        var repository = Substitute.For<IMotorcycleRepository>();
        repository.GetByIdAsync("1", Arg.Any<CancellationToken>()).Returns(motorcycle);

        var useCase = new UpdateMotorcycleLicensePlateUseCase(
            Substitute.For<ILogger<UpdateMotorcycleLicensePlateUseCase>>(),
            repository);

        var input = new UpdateMotorcycleLicensePlateInput("1", newLicensePlate);

        // Act
        var result = await useCase.Handle(input, CancellationToken.None);

        // Assert
        result.IsValid.Should().BeTrue();
        motorcycle.LicensePlate.Value.Should().Be(newLicensePlate);
        await repository.Received(originalLicensePlate != newLicensePlate ? 1 : 0)
            .SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ShouldReturnNotFound_WhenMotorcycleDoesNotExist()
    {
        // Arrange
        var repository = Substitute.For<IMotorcycleRepository>();
        repository.GetByIdAsync("1", Arg.Any<CancellationToken>()).Returns((Motorcycle?)null);

        var useCase = new UpdateMotorcycleLicensePlateUseCase(
            Substitute.For<ILogger<UpdateMotorcycleLicensePlateUseCase>>(),
            repository);

        var input = new UpdateMotorcycleLicensePlateInput("1", "XYZ-5678");

        // Act
        var result = await useCase.Handle(input, CancellationToken.None);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain("Motorcycle with Id '1' was not found.");
    }

    [Fact]
    public async Task Handle_ShouldReturnBadRequest_WhenLicensePlateAlreadyExist()
    {
        var motorcycleToBeUpdated = new Motorcycle("1", new LicensePlate("ABC-1234"), 2023, "ModelX");
        var request = new UpdateMotorcycleLicensePlateInput("1", "XYZ-789");

        var repository = Substitute.For<IMotorcycleRepository>();

        repository.GetByIdAsync("1", Arg.Any<CancellationToken>())
            .Returns(motorcycleToBeUpdated);
        repository.ExistsMotorcycleWithLicensePlateAsync(
            request.LicensePlate,
            Arg.Any<CancellationToken>())
            .Returns(true);

        var useCase = new UpdateMotorcycleLicensePlateUseCase(
            Substitute.For<ILogger<UpdateMotorcycleLicensePlateUseCase>>(),
            repository);

        // Act
        var result = await useCase.Handle(request, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.IsInvalid.Should().BeTrue();
        result.Errors.Should().Contain("The provided license plate already exists. Please use a unique license plate.");

        await repository.Received(1)
            .ExistsMotorcycleWithLicensePlateAsync(
            request.LicensePlate,
            Arg.Any<CancellationToken>());
        await repository.DidNotReceive().SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
