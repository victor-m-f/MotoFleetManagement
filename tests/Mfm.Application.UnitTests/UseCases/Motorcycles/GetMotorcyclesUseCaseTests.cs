using FluentAssertions;
using Mfm.Application.UseCases.Motorcycles.GetMotorcycles;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.ValueObjects;
using Mfm.Domain.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Mfm.Application.UnitTests.UseCases.Motorcycles;
public sealed class GetMotorcyclesUseCaseTests
{
    [Fact]
    public async Task Handle_ShouldReturnMotorcycles_WhenCalledWithValidData()
    {
        // Arrange
        var motorcycles = new List<Motorcycle>
        {
            new("1", new LicensePlate("ABC12345"), 2024, "ModelX"),
            new("2", new LicensePlate("CBA54321"), 2022, "ModelY"),
        };

        var repository = Substitute.For<IMotorcycleRepository>();
        repository.GetMotorcyclesAsync(null, Arg.Any<CancellationToken>())
                  .Returns(motorcycles);

        var useCase = new GetMotorcyclesUseCase(
            Substitute.For<ILogger<GetMotorcyclesUseCase>>(),
            repository);

        // Act
        var result = await useCase.Handle(new GetMotorcyclesInput(null), CancellationToken.None);

        // Assert
        result.Motorcycles.Should().BeEquivalentTo(
            motorcycles,
            options =>
            options.ComparingByMembers<Motorcycle>()
            .WithTracing() 
            .Excluding(m => m.LicensePlate)
            .Excluding(m => m.Rentals));

        await repository.Received(1).GetMotorcyclesAsync(null, Arg.Any<CancellationToken>());
    }

    [Theory]
    [InlineData("ABC12345", 1)]
    [InlineData("CBA54321", 1)]
    [InlineData("NOTEXIST", 0)]
    public async Task Handle_ShouldReturnFilteredMotorcycles_WhenLicensePlateIsProvided(string licensePlate, int expectedCount)
    {
        // Arrange
        var motorcycles = new List<Motorcycle>
        {
            new("1", new LicensePlate("ABC12345"), 2024, "ModelX"),
            new("2", new LicensePlate("CBA54321"), 2022, "ModelY"),
        };

        var repository = Substitute.For<IMotorcycleRepository>();
        repository.GetMotorcyclesAsync(licensePlate, Arg.Any<CancellationToken>())
                  .Returns(motorcycles.Where(m => m.LicensePlate.Value == licensePlate).ToList());

        var useCase = new GetMotorcyclesUseCase(
            Substitute.For<ILogger<GetMotorcyclesUseCase>>(),
            repository);

        // Act
        var result = await useCase.Handle(new GetMotorcyclesInput(licensePlate), CancellationToken.None);

        // Assert
        result.Motorcycles.Should().HaveCount(expectedCount);
        if (expectedCount > 0)
        {
            result.Motorcycles.First().LicensePlate.Should().Be(licensePlate);
        }

        await repository.Received(1).GetMotorcyclesAsync(licensePlate, Arg.Any<CancellationToken>());
    }
}
