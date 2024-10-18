using Mfm.Domain.Entities.Enums;
using Mfm.Domain.Exceptions;

namespace Mfm.Domain.Entities.ValueObjects;
public sealed class Cnh : IEquatable<Cnh>
{
    public string Number { get; }
    public CnhType Type { get; }

    public Cnh(string number, CnhType type)
    {
        if (string.IsNullOrWhiteSpace(number) || number.Length != 11)
        {
            throw new ValidationException();
        }

        if (!IsValidCnhType(type))
        {
            throw new ValidationException();
        }

        Number = number;
        Type = type;
    }

    public override bool Equals(object? obj) => obj is Cnh other && Equals(other);

    public bool Equals(Cnh? other) => Number == other?.Number && Type == other.Type;

    public override int GetHashCode() => HashCode.Combine(Number, Type);

    public override string ToString() => $"{Type} - {Number}";

    private static bool IsValidCnhType(CnhType type) =>
        type == CnhType.A || type == CnhType.B || type == CnhType.AB;
}