using FluentAssertions;
using Mfm.Application.UseCases.Motorcycles.GetMotorcycleById;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.ValueObjects;
using Mfm.Domain.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Mfm.Application.UnitTests.UseCases.Motorcycles;
public sealed class GetMotorcycleByIdUseCaseTests
{
    [Fact]
    public async Task Handle_ShouldReturnMotorcycle_WhenCalledWithValidId()
    {
        // Arrange
        var motorcycle = new Motorcycle("1", new LicensePlate("ABC12345"), 2024, "ModelX");

        var repository = Substitute.For<IMotorcycleRepository>();
        repository.GetByIdAsync(motorcycle.Id, Arg.Any<CancellationToken>())
                  .Returns(motorcycle);

        var useCase = new GetMotorcycleByIdUseCase(
            Substitute.For<ILogger<GetMotorcycleByIdUseCase>>(),
            repository);

        // Act
        var result = await useCase.Handle(new GetMotorcycleByIdInput(motorcycle.Id), CancellationToken.None);

        // Assert
        result.Motorcycle.Should().BeEquivalentTo(
            motorcycle,
            options =>
            options.ComparingByMembers<Motorcycle>()
            .WithTracing()
            .Excluding(m => m.LicensePlate));
        await repository.Received(1).GetByIdAsync(motorcycle.Id, Arg.Any<CancellationToken>());
    }
}
