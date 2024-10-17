using FluentAssertions;
using MassTransit;
using Mfm.Application.Dtos.Motorcycles;
using Mfm.Application.UseCases.Motorcycles.CreateMotorcycle;
using Mfm.Domain.Entities;
using Mfm.Domain.Events;
using Mfm.Domain.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Net;

namespace Mfm.Application.UnitTests.UseCases.Motorcycles;
public sealed class CreateMotorcycleUseCaseTests
{
    [Fact]
    public async Task Handle_ShouldAddMotorcycleAndPublishEvent_WhenCalledWithValidData()
    {
        // Arrange
        var repository = Substitute.For<IMotorcycleRepository>();
        var publisher = Substitute.For<IPublishEndpoint>();

        var request = new CreateMotorcycleInput(
            new MotorcycleDto
            {
                Id = "motorcycle-id",
                LicensePlate = "ABC-1234",
                Year = 2022,
                Model = "Test Model"
            });

        var useCase = new CreateMotorcycleUseCase(
            Substitute.For<ILogger<CreateMotorcycleUseCase>>(),
            repository,
            publisher);
        var cancellationToken = new CancellationToken();

        // Act
        var result = await useCase.Handle(request, cancellationToken);

        // Assert
        repository.Received(1).Add(Arg.Is<Motorcycle>(x =>
            x.Id == request.Motorcycle.Id &&
            x.LicensePlate.Value == request.Motorcycle.LicensePlate &&
            x.Year == request.Motorcycle.Year &&
            x.Model == request.Motorcycle.Model));

        await repository.Received(1)
            .SaveChangesAsync(cancellationToken);

        await publisher.Received(1).Publish(
            Arg.Is<MotorcycleCreatedEvent>(x =>
                x.MotorcycleId == request.Motorcycle.Id &&
                x.LicensePlate == request.Motorcycle.LicensePlate &&
                x.Year == request.Motorcycle.Year &&
                x.Model == request.Motorcycle.Model),
            cancellationToken);

        result.Should().BeOfType<CreateMotorcycleOutput>();
        result.HttpStatusCode.Should().Be(HttpStatusCode.Created);
    }
}
