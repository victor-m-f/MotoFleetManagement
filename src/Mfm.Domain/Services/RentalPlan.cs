using Mfm.Domain.Entities.Enums;

namespace Mfm.Domain.Services;
public sealed class RentalPlan
{
    public RentalPlanType Type { get; }
    public int DurationInDays { get; }
    public decimal DailyRate { get; }

    private RentalPlan(RentalPlanType type, int durationInDays, decimal dailyRate)
    {
        Type = type;
        DurationInDays = durationInDays;
        DailyRate = dailyRate;
    }

    // This constructor is used by EF Core
    private RentalPlan() { }

    public static RentalPlan SevenDays => new(RentalPlanType.SevenDays, 7, 30m);
    public static RentalPlan FifteenDays => new(RentalPlanType.FifteenDays, 15, 28m);
    public static RentalPlan ThirtyDays => new(RentalPlanType.ThirtyDays, 30, 22m);
    public static RentalPlan FortyFiveDays => new(RentalPlanType.FortyFiveDays, 45, 20m);
    public static RentalPlan FiftyDays => new(RentalPlanType.FiftyDays, 50, 18m);

    public static RentalPlan GetPlan(RentalPlanType type) => type switch
    {
        RentalPlanType.SevenDays => SevenDays,
        RentalPlanType.FifteenDays => FifteenDays,
        RentalPlanType.ThirtyDays => ThirtyDays,
        RentalPlanType.FortyFiveDays => FortyFiveDays,
        RentalPlanType.FiftyDays => FiftyDays,
        _ => throw new ArgumentOutOfRangeException(nameof(type), $"Invalid rental plan type: {type}")
    };
}