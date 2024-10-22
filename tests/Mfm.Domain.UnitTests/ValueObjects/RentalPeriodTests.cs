using FluentAssertions;
using Mfm.Domain.Entities.ValueObjects;
using Mfm.Domain.Exceptions;

namespace Mfm.Domain.UnitTests.ValueObjects;
public sealed class RentalPeriodTests
{
    private readonly TimeProvider _timeProvider;

    public RentalPeriodTests()
    {
        _timeProvider = TimeProvider.System;
    }

    [Fact]
    public void Constructor_ShouldCreateRentalPeriod_WhenValidParameters()
    {
        // Arrange
        var creationDate = _timeProvider.GetLocalNow();
        var expectedStartDate = creationDate.Date.AddDays(1);
        var expectedEndDate = expectedStartDate.AddDays(7);
        var endDate = expectedEndDate;

        // Act
        var rentalPeriod = new RentalPeriod(expectedStartDate, endDate, expectedEndDate, _timeProvider);

        // Assert
        rentalPeriod.StartDate.Should().Be(expectedStartDate);
        rentalPeriod.ExpectedEndDate.Should().Be(expectedEndDate);
        rentalPeriod.EndDate.Should().Be(endDate);
    }

    [Fact]
    public void Constructor_ShouldThrowValidationException_WhenStartDateIsNotDayAfterCreation()
    {
        // Arrange
        var creationDate = _timeProvider.GetLocalNow();
        var invalidStartDate = creationDate.Date;
        var expectedEndDate = invalidStartDate.AddDays(7);
        var endDate = expectedEndDate;

        // Act
        var action = () => new RentalPeriod(invalidStartDate, endDate, expectedEndDate, _timeProvider);

        // Assert
        action.Should().Throw<ValidationException>()
            .WithMessage($"Start date must be {creationDate.Date.AddDays(1):yyyy-MM-dd}.");
    }

    [Fact]
    public void Constructor_ShouldThrowValidationException_WhenStartDateIsAfterExpectedEndDate()
    {
        // Arrange
        var creationDate = _timeProvider.GetLocalNow();
        var expectedStartDate = creationDate.Date.AddDays(1);
        var expectedEndDate = expectedStartDate.AddDays(-1);
        var endDate = expectedStartDate.AddDays(1);

        // Act
        var action = () => new RentalPeriod(expectedStartDate, endDate, expectedEndDate, _timeProvider);

        // Assert
        action.Should().Throw<ValidationException>()
            .WithMessage($"Start date ({expectedStartDate:yyyy-MM-dd}) must be before expected end date ({expectedEndDate:yyyy-MM-dd}).");
    }

    [Fact]
    public void Constructor_ShouldThrowValidationException_WhenStartDateIsAfterEndDate()
    {
        // Arrange
        var creationDate = _timeProvider.GetLocalNow();
        var expectedStartDate = creationDate.Date.AddDays(1);
        var endDate = expectedStartDate.AddDays(-1);
        var expectedEndDate = expectedStartDate.AddDays(7);

        // Act
        var action = () => new RentalPeriod(expectedStartDate, endDate, expectedEndDate, _timeProvider);

        // Assert
        action.Should().Throw<ValidationException>()
            .WithMessage($"Start date ({expectedStartDate:yyyy-MM-dd}) must be before end date ({endDate:yyyy-MM-dd}).");
    }

    [Fact]
    public void Equals_ShouldReturnTrue_WhenRentalPeriodsAreEqual()
    {
        // Arrange
        var creationDate = _timeProvider.GetLocalNow();
        var startDate = creationDate.Date.AddDays(1);
        var expectedEndDate = startDate.AddDays(7);
        var endDate = expectedEndDate;

        var rentalPeriod1 = new RentalPeriod(startDate, endDate, expectedEndDate, _timeProvider);
        var rentalPeriod2 = new RentalPeriod(startDate, endDate, expectedEndDate, _timeProvider);

        // Act & Assert
        rentalPeriod1.Should().Be(rentalPeriod2);
        (rentalPeriod1 == rentalPeriod2).Should().BeTrue();
    }

    [Fact]
    public void Equals_ShouldReturnFalse_WhenRentalPeriodsAreNotEqual()
    {
        // Arrange
        var creationDate = _timeProvider.GetLocalNow();
        var startDate = creationDate.Date.AddDays(1);
        var expectedEndDate = startDate.AddDays(7);
        var endDate1 = expectedEndDate;
        var endDate2 = expectedEndDate.AddDays(1);

        var rentalPeriod1 = new RentalPeriod(startDate, endDate1, expectedEndDate, _timeProvider);
        var rentalPeriod2 = new RentalPeriod(startDate, endDate2, expectedEndDate, _timeProvider);

        // Act & Assert
        rentalPeriod1.Should().NotBe(rentalPeriod2);
        (rentalPeriod1 != rentalPeriod2).Should().BeTrue();
    }
}