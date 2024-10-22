using FluentAssertions;
using Mfm.Application.Dtos.Rentals;
using Mfm.Application.Helpers;
using Mfm.Application.UseCases.Rentals.CreateRental;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.Enums;
using Mfm.Domain.Entities.ValueObjects;
using Mfm.Domain.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Net;

namespace Mfm.Application.UnitTests.UseCases.Rentals;
public sealed class CreateRentalUseCaseTests
{
    [Fact]
    public async Task Handle_ShouldCreateRental_WhenCalledWithValidData()
    {
        // Arrange
        var deliveryPersonRepository = Substitute.For<IDeliveryPersonRepository>();
        var motorcycleRepository = Substitute.For<IMotorcycleRepository>();
        var rentalRepository = Substitute.For<IRentalRepository>();
        var timeProvider = TimeProvider.System;
        var logger = Substitute.For<ILogger<CreateRentalUseCase>>();

        var deliveryPerson = new DeliveryPerson(
            id: "deliveryperson-id",
            name: "John Doe",
            cnpj: new Cnpj("12345678000195"),
            dateOfBirth: new DateTime(1990, 5, 20),
            cnh: new Cnh("12345678910", CnhType.A),
            cnhImageUrl: "cnh-image-url");

        var motorcycle = new Motorcycle(
            id: "motorcycle-id",
            licensePlate: new LicensePlate("ABC-1234"),
            year: 2022,
            model: "Test Model");

        var request = new CreateRentalInput(
            new CreateRentalDto
            {
                DeliveryPersonId = deliveryPerson.Id,
                MotorcycleId = motorcycle.Id,
                StartDate = DateTime.Now.Date.AddDays(1).ToString(),
                EndDate = DateTime.Now.Date.AddDays(7).ToString(),
                ExpectedEndDate = DateTime.Now.Date.AddDays(7).ToString(),
                Plan = 7
            });

        var useCase = new CreateRentalUseCase(
            logger,
            deliveryPersonRepository,
            motorcycleRepository,
            rentalRepository,
            timeProvider);

        var cancellationToken = new CancellationToken();

        deliveryPersonRepository
            .GetByIdAsync(deliveryPerson.Id, cancellationToken)
            .Returns(deliveryPerson);

        motorcycleRepository
            .GetByIdAsync(motorcycle.Id, true, cancellationToken)
            .Returns(motorcycle);

        // Act
        var result = await useCase.Handle(request, cancellationToken);

        // Assert
        rentalRepository.Received(1).Add(Arg.Is<Rental>(x =>
            x.DeliveryPersonId == request.Rental.DeliveryPersonId &&
            x.MotorcycleId == request.Rental.MotorcycleId &&
            x.PlanType == (RentalPlanType)request.Rental.Plan &&
            x.Period.StartDate == request.Rental.StartDate.ToDateTime() &&
            x.Period.EndDate == request.Rental.EndDate.ToDateTime() &&
            x.Period.ExpectedEndDate == request.Rental.ExpectedEndDate.ToDateTime()));

        await rentalRepository.Received(1)
            .SaveChangesAsync(cancellationToken);

        result.Should().BeOfType<CreateRentalOutput>();
        result.HttpStatusCode.Should().Be(System.Net.HttpStatusCode.Created);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenDeliveryPersonNotFound()
    {
        // Arrange
        var deliveryPersonRepository = Substitute.For<IDeliveryPersonRepository>();
        var motorcycleRepository = Substitute.For<IMotorcycleRepository>();
        var rentalRepository = Substitute.For<IRentalRepository>();
        var timeProvider = TimeProvider.System;
        var logger = Substitute.For<ILogger<CreateRentalUseCase>>();

        var request = new CreateRentalInput(
            new CreateRentalDto
            {
                DeliveryPersonId = "non-existent-deliveryperson-id",
                MotorcycleId = "motorcycle-id",
                StartDate = DateTime.Now.Date.AddDays(1).ToString(),
                EndDate = DateTime.Now.Date.AddDays(7).ToString(),
                ExpectedEndDate = DateTime.Now.Date.AddDays(7).ToString(),
                Plan = 7
            });

        var useCase = new CreateRentalUseCase(
            logger,
            deliveryPersonRepository,
            motorcycleRepository,
            rentalRepository,
            timeProvider);

        var cancellationToken = new CancellationToken();

        deliveryPersonRepository
            .GetByIdAsync(request.Rental.DeliveryPersonId, cancellationToken)
            .Returns(Task.FromResult(default(DeliveryPerson)));

        // Act
        var result = await useCase.Handle(request, cancellationToken);

        // Assert
        result.Should().BeOfType<CreateRentalOutput>();
        result.IsValid.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenDeliveryPersonNotEligible()
    {
        // Arrange
        var deliveryPersonRepository = Substitute.For<IDeliveryPersonRepository>();
        var motorcycleRepository = Substitute.For<IMotorcycleRepository>();
        var rentalRepository = Substitute.For<IRentalRepository>();
        var timeProvider = TimeProvider.System;
        var logger = Substitute.For<ILogger<CreateRentalUseCase>>();

        var deliveryPerson = new DeliveryPerson(
            id: "deliveryperson-id",
            name: "John Doe",
            cnpj: new Cnpj("12345678000195"),
            dateOfBirth: new DateTime(1990, 5, 20),
            cnh: new Cnh("12345678910", CnhType.B), // Not motorcycle driver
            cnhImageUrl: "cnh-image-url");

        var request = new CreateRentalInput(
            new CreateRentalDto
            {
                DeliveryPersonId = deliveryPerson.Id,
                MotorcycleId = "motorcycle-id",
                StartDate = DateTime.Now.Date.AddDays(1).ToString(),
                EndDate = DateTime.Now.Date.AddDays(7).ToString(),
                ExpectedEndDate = DateTime.Now.Date.AddDays(7).ToString(),
                Plan = 7
            });

        var useCase = new CreateRentalUseCase(
            logger,
            deliveryPersonRepository,
            motorcycleRepository,
            rentalRepository,
            timeProvider);

        var cancellationToken = new CancellationToken();

        deliveryPersonRepository
            .GetByIdAsync(deliveryPerson.Id, cancellationToken)!
            .Returns(Task.FromResult(deliveryPerson));

        // Act
        var result = await useCase.Handle(request, cancellationToken);

        // Assert
        result.Should().BeOfType<CreateRentalOutput>();
        result.IsValid.Should().BeFalse();
        result.HttpStatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenMotorcycleNotFound()
    {
        // Arrange
        var deliveryPersonRepository = Substitute.For<IDeliveryPersonRepository>();
        var motorcycleRepository = Substitute.For<IMotorcycleRepository>();
        var rentalRepository = Substitute.For<IRentalRepository>();
        var timeProvider = TimeProvider.System;
        var logger = Substitute.For<ILogger<CreateRentalUseCase>>();

        var deliveryPerson = new DeliveryPerson(
            id: "deliveryperson-id",
            name: "John Doe",
            cnpj: new Cnpj("12345678000195"),
            dateOfBirth: new DateTime(1990, 5, 20),
            cnh: new Cnh("12345678910", CnhType.A),
            cnhImageUrl: "cnh-image-url");

        var request = new CreateRentalInput(
            new CreateRentalDto
            {
                DeliveryPersonId = deliveryPerson.Id,
                MotorcycleId = "non-existent-motorcycle-id",
                StartDate = DateTime.Now.Date.AddDays(1).ToString(),
                EndDate = DateTime.Now.Date.AddDays(7).ToString(),
                ExpectedEndDate = DateTime.Now.Date.AddDays(7).ToString(),
                Plan = 7
            });

        var useCase = new CreateRentalUseCase(
            logger,
            deliveryPersonRepository,
            motorcycleRepository,
            rentalRepository,
            timeProvider);

        var cancellationToken = new CancellationToken();

        deliveryPersonRepository
            .GetByIdAsync(deliveryPerson.Id, cancellationToken)
            .Returns(deliveryPerson);

        motorcycleRepository
            .GetByIdAsync(request.Rental.MotorcycleId, true, cancellationToken)!
            .Returns(Task.FromResult(default(Motorcycle)));

        // Act
        var result = await useCase.Handle(request, cancellationToken);

        // Assert
        result.Should().BeOfType<CreateRentalOutput>();
        result.IsValid.Should().BeFalse();
        result.HttpStatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Handle_ShouldReturnError_WhenMotorcycleIsUnavailable()
    {
        // Arrange
        var deliveryPersonRepository = Substitute.For<IDeliveryPersonRepository>();
        var motorcycleRepository = Substitute.For<IMotorcycleRepository>();
        var rentalRepository = Substitute.For<IRentalRepository>();
        var timeProvider = TimeProvider.System;
        var logger = Substitute.For<ILogger<CreateRentalUseCase>>();

        var deliveryPerson = new DeliveryPerson(
            id: "deliveryperson-id",
            name: "John Doe",
            cnpj: new Cnpj("12345678000195"),
            dateOfBirth: new DateTime(1990, 5, 20),
            cnh: new Cnh("12345678910", CnhType.A),
            cnhImageUrl: "cnh-image-url");

        var motorcycle = new Motorcycle(
                id: "motorcycle-id",
                licensePlate: new LicensePlate("ABC-1234"),
                year: 2022,
                model: "Test Model");

        motorcycle.Rentals.Add(
            new Rental(
                motorcycle.Id,
                deliveryPerson.Id,
                RentalPlanType.SevenDays,
                DateTime.Now.Date.AddDays(1),
                DateTime.Now.Date.AddDays(7),
                DateTime.Now.Date.AddDays(7),
                timeProvider));

        var request = new CreateRentalInput(
            new CreateRentalDto
            {
                DeliveryPersonId = deliveryPerson.Id,
                MotorcycleId = "motorcycle-id",
                StartDate = DateTime.Now.Date.AddDays(1).ToString(),
                EndDate = DateTime.Now.Date.AddDays(7).ToString(),
                ExpectedEndDate = DateTime.Now.Date.AddDays(7).ToString(),
                Plan = 7,
            });

        var useCase = new CreateRentalUseCase(
            logger,
            deliveryPersonRepository,
            motorcycleRepository,
            rentalRepository,
            timeProvider);

        var cancellationToken = new CancellationToken();

        deliveryPersonRepository
            .GetByIdAsync(deliveryPerson.Id, cancellationToken)
            .Returns(deliveryPerson);

        motorcycleRepository
            .GetByIdAsync(request.Rental.MotorcycleId, true, cancellationToken)
            .Returns(motorcycle);

        // Act
        var result = await useCase.Handle(request, cancellationToken);

        // Assert
        result.Should().BeOfType<CreateRentalOutput>();
        result.IsValid.Should().BeFalse();
        result.HttpStatusCode.Should().Be(HttpStatusCode.Conflict);
    }
}