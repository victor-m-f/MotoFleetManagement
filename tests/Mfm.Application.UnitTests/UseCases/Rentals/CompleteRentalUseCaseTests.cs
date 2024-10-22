using FluentAssertions;
using Mfm.Application.UseCases.Rentals.CompleteRental;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.Enums;
using Mfm.Domain.Repositories;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Mfm.Application.UnitTests.UseCases.Rentals;
public sealed class CompleteRentalUseCaseTests
{
    [Fact]
    public async Task Handle_ShouldCalculateTotalCost_ForEarlyReturn()
    {
        // Arrange
        var rentalRepository = Substitute.For<IRentalRepository>();
        var logger = Substitute.For<ILogger<CompleteRentalUseCase>>();

        var rental = new Rental(
            motorcycleId: "motorcycle-id",
            deliveryPersonId: "deliveryperson-id",
            planType: RentalPlanType.SevenDays,
            startDate: DateTimeOffset.Now.Date.AddDays(1),
            endDate: DateTimeOffset.Now.Date.AddDays(7),
            expectedEndDate: DateTimeOffset.Now.Date.AddDays(7),
            timeProvider: TimeProvider.System);

        rentalRepository.GetByIdAsync(rental.Id, CancellationToken.None)
            .Returns(rental);

        var useCase = new CompleteRentalUseCase(logger, rentalRepository);

        var input = new CompleteRentalInput(
            rental.Id,
            DateTimeOffset.Now.Date.AddDays(3).ToString("yyyy-MM-ddTHH:mm:ssZ"));

        // Act
        var result = await useCase.Handle(input, CancellationToken.None);

        // Assert
        result.IsValid.Should().BeTrue();
    }
}