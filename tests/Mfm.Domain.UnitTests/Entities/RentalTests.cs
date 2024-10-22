using FluentAssertions;
using Mfm.Domain.Entities;
using Mfm.Domain.Entities.Enums;
using Mfm.Domain.Exceptions;

namespace Mfm.Domain.UnitTests.Entities;
public sealed class RentalTests
{
    private readonly TimeProvider _timeProvider;

    public RentalTests()
    {
        _timeProvider = TimeProvider.System;
    }

    [Fact]
    public void Constructor_ShouldCreateRental_WhenValidParameters()
    {
        // Arrange
        var motorcycleId = "motorcycle123";
        var deliveryPersonId = "deliveryPerson123";
        var planType = RentalPlanType.SevenDays;
        var creationDate = _timeProvider.GetLocalNow();
        var startDate = creationDate.Date.AddDays(1);
        var expectedEndDate = startDate.AddDays((int)planType);
        var endDate = expectedEndDate;

        // Act
        var rental = new Rental(
            motorcycleId,
            deliveryPersonId,
            planType,
            startDate,
            endDate,
            expectedEndDate,
            _timeProvider);

        // Assert
        rental.MotorcycleId.Should().Be(motorcycleId);
        rental.DeliveryPersonId.Should().Be(deliveryPersonId);
        rental.PlanType.Should().Be(planType);
        rental.Period.StartDate.Should().Be(startDate);
        rental.Period.ExpectedEndDate.Should().Be(expectedEndDate);
        rental.Period.EndDate.Should().Be(endDate);
        rental.TotalCost.Should().Be(rental.CalculateTotalCost());
    }

    [Fact]
    public void Constructor_ShouldThrowValidationException_WhenRentalPeriodIsInvalid()
    {
        // Arrange
        var motorcycleId = "motorcycle123";
        var deliveryPersonId = "deliveryPerson123";
        var planType = RentalPlanType.SevenDays;
        var creationDate = _timeProvider.GetLocalNow();
        var invalidStartDate = creationDate.Date.AddDays(-1); // Invalid start date
        var expectedEndDate = invalidStartDate.AddDays((int)planType);
        var endDate = expectedEndDate;

        // Act
        var action = () => new Rental(
            motorcycleId,
            deliveryPersonId,
            planType,
            invalidStartDate,
            endDate,
            expectedEndDate,
            _timeProvider);

        // Assert
        action.Should().Throw<ValidationException>()
            .WithMessage($"Start date must be {creationDate.Date.AddDays(1):yyyy-MM-dd}.");
    }

    [Fact]
    public void CompleteRental_ShouldUpdateEndDateAndTotalCost_WhenValidActualEndDate()
    {
        // Arrange
        var motorcycleId = "motorcycle123";
        var deliveryPersonId = "deliveryPerson123";
        var planType = RentalPlanType.SevenDays;
        var creationDate = _timeProvider.GetLocalNow();
        var startDate = creationDate.Date.AddDays(1);
        var expectedEndDate = startDate.AddDays((int)planType);
        var endDate = expectedEndDate;
        var rental = new Rental(
            motorcycleId,
            deliveryPersonId,
            planType,
            startDate,
            endDate,
            expectedEndDate,
            _timeProvider);

        var actualEndDate = endDate.AddDays(1);

        // Act
        rental.CompleteRental(actualEndDate);

        // Assert
        rental.ReturnDate.Should().Be(actualEndDate);
        rental.TotalCost.Should().Be(rental.CalculateTotalCost());
    }

    [Fact]
    public void CompleteRental_ShouldThrowValidationException_WhenActualEndDateIsBeforeStartDate()
    {
        // Arrange
        var motorcycleId = "motorcycle123";
        var deliveryPersonId = "deliveryPerson123";
        var planType = RentalPlanType.SevenDays;
        var creationDate = _timeProvider.GetLocalNow();
        var startDate = creationDate.Date.AddDays(1);
        var expectedEndDate = startDate.AddDays((int)planType);
        var endDate = expectedEndDate;
        var rental = new Rental(
            motorcycleId,
            deliveryPersonId,
            planType,
            startDate,
            endDate,
            expectedEndDate,
            _timeProvider);

        var invalidActualEndDate = startDate.AddDays(-1);

        // Act
        var action = () => rental.CompleteRental(invalidActualEndDate);

        // Assert
        action.Should().Throw<ValidationException>()
            .WithMessage("Actual end date cannot be before start date.");
    }

    [Theory]
    [InlineData(RentalPlanType.SevenDays, 7, 30)]
    [InlineData(RentalPlanType.FifteenDays, 15, 28)]
    [InlineData(RentalPlanType.ThirtyDays, 30, 22)]
    public void CalculateTotalCost_ShouldReturnCorrectTotal_WhenActualDurationEqualsExpected(
        RentalPlanType planType,
        int duration,
        decimal dailyRate)
    {
        // Arrange
        var motorcycleId = "motorcycle123";
        var deliveryPersonId = "deliveryPerson123";
        var creationDate = _timeProvider.GetLocalNow();
        var startDate = creationDate.Date.AddDays(1);
        var expectedEndDate = startDate.AddDays(duration);
        var endDate = expectedEndDate;

        var rental = new Rental(
            motorcycleId,
            deliveryPersonId,
            planType,
            startDate,
            endDate,
            expectedEndDate,
            _timeProvider);

        // Act
        var totalCost = rental.CalculateTotalCost();

        // Assert
        totalCost.Should().Be(dailyRate * duration);
    }

    [Fact]
    public void CalculateTotalCost_ShouldIncludeFine_WhenActualDurationIsLessThanExpected_ForSevenDaysPlan()
    {
        // Arrange
        var planType = RentalPlanType.SevenDays;
        var duration = 7;
        var dailyRate = 30m;
        var motorcycleId = "motorcycle123";
        var deliveryPersonId = "deliveryPerson123";
        var creationDate = _timeProvider.GetLocalNow();
        var startDate = creationDate.Date.AddDays(1);
        var expectedEndDate = startDate.AddDays(duration);
        var endDate = startDate.AddDays(5); // Returned 2 days early

        var rental = new Rental(
            motorcycleId,
            deliveryPersonId,
            planType,
            startDate,
            endDate,
            expectedEndDate,
            _timeProvider);

        // Act
        var totalCost = rental.CalculateTotalCost();

        // Fine is 20% of the unused days
        var daysUsed = 5;
        var unusedDays = duration - daysUsed; // 2 days
        var baseCost = dailyRate * daysUsed;
        var fine = 0.20m * (dailyRate * unusedDays);
        var expectedTotal = baseCost + fine;

        // Assert
        totalCost.Should().Be(expectedTotal);
    }

    [Fact]
    public void CalculateTotalCost_ShouldIncludeExtraCharges_WhenActualDurationIsGreaterThanExpected()
    {
        // Arrange
        var planType = RentalPlanType.SevenDays;
        var duration = 7;
        var dailyRate = 30m;
        var motorcycleId = "motorcycle123";
        var deliveryPersonId = "deliveryPerson123";
        var creationDate = _timeProvider.GetLocalNow();
        var startDate = creationDate.Date.AddDays(1);
        var expectedEndDate = startDate.AddDays(duration);
        var endDate = startDate.AddDays(9); // Returned 2 days late

        var rental = new Rental(
            motorcycleId,
            deliveryPersonId,
            planType,
            startDate,
            endDate,
            expectedEndDate,
            _timeProvider);

        // Act
        var totalCost = rental.CalculateTotalCost();

        // Extra charges are $50 per extra day
        var extraDays = 2;
        var baseCost = dailyRate * duration;
        var extraCharges = extraDays * 50m;
        var expectedTotal = baseCost + extraCharges;

        // Assert
        totalCost.Should().Be(expectedTotal);
    }
}