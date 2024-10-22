using FluentAssertions;
using Mfm.Application.Dtos.Rentals;
using Mfm.Application.UseCases.Rentals.GetRentalById;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.Enums;
using Mfm.Domain.Repositories;
using Mfm.Domain.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace Mfm.Application.UnitTests.UseCases.Rentals;
public sealed class GetRentalByIdUseCaseTests
{
    [Fact]
    public async Task Handle_ShouldReturnRental_WhenCalledWithValidId()
    {
        // Arrange
        var rental = new Rental(
            motorcycleId: "motorcycle-id",
            deliveryPersonId: "deliveryperson-id",
            planType: RentalPlanType.SevenDays,
            startDate: DateTimeOffset.Now.AddDays(1),
            endDate: DateTimeOffset.Now.AddDays(7),
            expectedEndDate: DateTimeOffset.Now.AddDays(7),
            timeProvider: TimeProvider.System);

        var plan = RentalPlan.GetPlan(rental.PlanType);
        var expectedRentalDto = new RentalDto
        {
            Id = rental.Id,
            DailyRate = plan.DailyRate,
            DeliveryPersonId = rental.DeliveryPersonId,
            MotorcycleId = rental.MotorcycleId,
            StartDate = rental.Period.StartDate,
            EndDate = rental.Period.EndDate,
            ExpectedEndDate = rental.Period.ExpectedEndDate,
            ReturnDate = rental.ReturnDate,
        };

        var repository = Substitute.For<IRentalRepository>();
        repository.GetByIdAsync(rental.Id, cancellationToken: Arg.Any<CancellationToken>())
                  .Returns(rental);

        var useCase = new GetRentalByIdUseCase(
            Substitute.For<ILogger<GetRentalByIdUseCase>>(),
            repository);

        // Act
        var result = await useCase.Handle(new GetRentalByIdInput(rental.Id), CancellationToken.None);

        // Assert
        result.Rental.Should().BeEquivalentTo(
            expectedRentalDto,
            options => options.ComparingByMembers<RentalDto>().WithTracing());

        await repository.Received(1)
            .GetByIdAsync(rental.Id, cancellationToken: Arg.Any<CancellationToken>());
    }
}
