namespace Mfm.Domain.ValueObjects;
public sealed class LicensePlate : IEquatable<LicensePlate>
{
    public string Value { get; init; }

    public LicensePlate(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || value.Length != 7)
        {
            throw new ArgumentException("License plate must be exactly 7 characters.", nameof(value));
        }

        Value = value;
    }

    public override bool Equals(object? obj) => obj is LicensePlate other && Equals(other);

    public bool Equals(LicensePlate? other) => Value == other?.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}
