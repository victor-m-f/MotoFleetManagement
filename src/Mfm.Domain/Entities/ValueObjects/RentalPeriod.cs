using Mfm.Domain.Exceptions;

namespace Mfm.Domain.Entities.ValueObjects;
public sealed class RentalPeriod : IEquatable<RentalPeriod>
{
    public DateTimeOffset StartDate { get; }
    public DateTimeOffset ExpectedEndDate { get; }
    public DateTimeOffset EndDate { get; private set; }

    public RentalPeriod(
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        DateTimeOffset expectedEndDate,
        TimeProvider timeProvider)
    {
        var creationDate = timeProvider.GetLocalNow();
        var expectedStartDate = creationDate.Date.AddDays(1);
        if (startDate.Date != expectedStartDate)
        {
            throw new ValidationException($"Start date must be {expectedStartDate:yyyy-MM-dd}.");
        }

        if (startDate >= expectedEndDate)
        {
            throw new ValidationException($"Start date ({startDate:yyyy-MM-dd}) must be before expected end date ({expectedEndDate:yyyy-MM-dd}).");
        }

        if (startDate >= endDate)
        {
            throw new ValidationException($"Start date ({startDate:yyyy-MM-dd}) must be before end date ({endDate:yyyy-MM-dd}).");
        }

        StartDate = startDate;
        ExpectedEndDate = expectedEndDate;
        EndDate = endDate;
    }

    // This constructor is used by EF Core
    private RentalPeriod() { }

    public void SetActualEndDate(DateTime actualEndDate)
    {
        if (actualEndDate < StartDate)
        {
            throw new ValidationException("Actual end date cannot be before start date.");
        }

        EndDate = actualEndDate;
    }

    public bool Equals(RentalPeriod? other)
    {
        if (other is null)
        {
            return false;
        }

        return StartDate == other.StartDate &&
               ExpectedEndDate == other.ExpectedEndDate &&
               EndDate == other.EndDate;
    }

    public override bool Equals(object? obj)
    {
        if (obj is null)
        {
            return false;
        }

        if (ReferenceEquals(this, obj))
        {
            return true;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return Equals((RentalPeriod)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StartDate, ExpectedEndDate, EndDate);
    }

    public static bool operator ==(RentalPeriod? left, RentalPeriod? right)
    {
        if (left is null)
        {
            return right is null;
        }

        return left.Equals(right);
    }

    public static bool operator !=(RentalPeriod? left, RentalPeriod? right)
    {
        return !(left == right);
    }
}
