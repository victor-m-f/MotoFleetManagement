namespace Mfm.Domain.Exceptions;
public sealed class ValidationException : Exception
{
    public ValidationException()
        : base("Dados inválidos")
    {
    }
}
