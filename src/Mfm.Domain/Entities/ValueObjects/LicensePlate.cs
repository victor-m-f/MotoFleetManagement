using Mfm.Domain.Entities.Rules;
using Mfm.Domain.Exceptions;

namespace Mfm.Domain.Entities.ValueObjects;
public sealed class LicensePlate : IEquatable<LicensePlate>
{
    public string Value { get; init; }

    public LicensePlate(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length != MotorcycleRules.LicensePlateMaxLength)
        {
            throw new ValidationException();
        }

        Value = value;
    }

    public override bool Equals(object? obj) => obj is LicensePlate other && Equals(other);

    public bool Equals(LicensePlate? other) => Value == other?.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
