using Mfm.Domain.Entities.Enums;
using Mfm.Domain.Entities.ValueObjects;
using Mfm.Domain.Services;

namespace Mfm.Domain.Entities;
public sealed class Rental
{
    public string Id { get; } = string.Empty;
    public string MotorcycleId { get; } = string.Empty;
    public string DeliveryPersonId { get; } = string.Empty;
    public RentalPlanType PlanType { get; }
    public RentalPeriod Period { get; private set; } = default!;
    public decimal TotalCost { get; private set; }
    public DateTimeOffset? ReturnDate { get; set; }

    public Motorcycle? Motorcycle { get; set; }
    public DeliveryPerson? DeliveryPerson { get; set; }

    public Rental(
        string motorcycleId,
        string deliveryPersonId,
        RentalPlanType planType,
        DateTimeOffset startDate,
        DateTimeOffset endDate,
        DateTimeOffset expectedEndDate,
        TimeProvider timeProvider)
    {
        Id = Guid.NewGuid().ToString();
        MotorcycleId = motorcycleId;
        DeliveryPersonId = deliveryPersonId;
        PlanType = planType;
        Period = new RentalPeriod(startDate, endDate, expectedEndDate, timeProvider);
        TotalCost = CalculateTotalCost();
    }

    // This constructor is used by EF Core
    private Rental() { }

    public void CompleteRental(DateTime actualEndDate)
    {
        Period.SetActualEndDate(actualEndDate);
        TotalCost = CalculateTotalCost();
    }

    public decimal CalculateTotalCost()
    {
        var plan = RentalPlan.GetPlan(PlanType);
        var dailyRate = plan.DailyRate;
        var expectedDuration = plan.DurationInDays;

        var actualDuration = (Period.EndDate.Date - Period.StartDate).Days;

        decimal totalCost;

        if (actualDuration == expectedDuration)
        {
            totalCost = dailyRate * expectedDuration;
        }
        else if (actualDuration < expectedDuration)
        {
            var daysUsed = actualDuration;
            var unusedDays = expectedDuration - daysUsed;
            var finePercentage = PlanType switch
            {
                RentalPlanType.SevenDays => 0.20m,
                RentalPlanType.FifteenDays => 0.40m,
                _ => 0m
            };

            var baseCost = dailyRate * daysUsed;
            var fine = finePercentage * (dailyRate * unusedDays);

            totalCost = baseCost + fine;
        }
        else
        {
            var extraDays = actualDuration - expectedDuration;
            var extraCharges = extraDays * 50m;
            var baseCost = dailyRate * expectedDuration;

            totalCost = baseCost + extraCharges;
        }

        return totalCost;
    }
}