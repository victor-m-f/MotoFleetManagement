namespace Mfm.Domain.Exceptions;
public sealed class ValidationException : Exception
{
    public ValidationException(
        string errorMessage = "Dados inválidos",
        Exception? innerException = null)
        : base(errorMessage, innerException)
    {
    }
}
