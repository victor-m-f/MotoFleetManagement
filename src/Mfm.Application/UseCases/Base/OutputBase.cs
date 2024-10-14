using System.Net;

namespace Mfm.Application.UseCases.Base;

public abstract class OutputBase
{
    public bool IsValid { get; protected set; }
    public HttpStatusCode HttpStatusCode { get; }
    public List<string> Errors { get; } = [];
    public bool IsInvalid => !IsValid;
    public int StatusCode => (int)HttpStatusCode;

    public OutputBase(HttpStatusCode httpStatusCode, bool isValid = true)
    {
        IsValid = isValid;
        HttpStatusCode = httpStatusCode;
    }

    public OutputBase(HttpStatusCode httpStatusCode, params string[] errorMessages)
    {
        IsValid = false;
        HttpStatusCode = httpStatusCode;
        AddErrors(errorMessages);
    }

    public void AddError(string errorMessage)
    {
        IsValid = false;
        Errors.Add(StandartizeErrorMessage(errorMessage));
    }

    public void AddErrors(IEnumerable<string> errorMessages)
    {
        IsValid = false;

        Errors.AddRange(errorMessages.Select(x => StandartizeErrorMessage(x)));
    }

    private static string StandartizeErrorMessage(string errorMessage)
    {
        if (string.IsNullOrEmpty(errorMessage))
        {
            return string.Empty;
        }

        errorMessage = errorMessage.Trim();

        if (!errorMessage.EndsWith('.'))
        {
            errorMessage += ".";
        }

        return errorMessage;
    }
}
