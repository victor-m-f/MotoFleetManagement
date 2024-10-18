using Mfm.Domain.Exceptions;

namespace Mfm.Domain.Entities.ValueObjects;
public sealed class Cnpj : IEquatable<Cnpj>
{
    public string Value { get; }

    public Cnpj(string value)
    {
        var cleanedValue = ClearNonDigits(value);
        if (!IsValid(cleanedValue))
        {
            throw new ValidationException();
        }
        Value = cleanedValue;
    }

    public override bool Equals(object? obj) => obj is Cnpj other && Equals(other);

    public bool Equals(Cnpj? other) => Value == other?.Value;

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;

    private static string ClearNonDigits(string value) =>
        new(value.Where(char.IsDigit).ToArray());

    private static bool IsValid(string cnpj)
    {
        if (string.IsNullOrWhiteSpace(cnpj) || cnpj.Length != 14)
        {
            return false;
        }

        var numbers = cnpj.Select(x => x - '0').ToArray();
        if (numbers.Distinct().Count() == 1)
        {
            return false;
        }

        static int CalculateChecksum(int[] digits, int[] multipliers)
        {
            var sum = 0;
            for (var i = 0; i < multipliers.Length; i++)
            {
                sum += digits[i] * multipliers[i];
            }
            var remainder = sum % 11;
            return remainder < 2 ? 0 : 11 - remainder;
        }

        var firstMultiplier = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        var secondMultiplier = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        var firstChecksum = CalculateChecksum(numbers.Take(12).ToArray(), firstMultiplier);
        var secondChecksum = CalculateChecksum(numbers.Take(13).ToArray(), secondMultiplier);

        return numbers[12] == firstChecksum && numbers[13] == secondChecksum;
    }
}
