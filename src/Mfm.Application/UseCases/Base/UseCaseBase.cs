using Microsoft.Extensions.Logging;

namespace Mfm.Application.UseCases.Base;

public abstract class UseCaseBase
{
    private const string UseCaseExecutionStartedLogMessage = "Execution started for {UseCaseName} with input: {@UseCaseInput}";
    private readonly ILogger _logger;

    protected UseCaseBase(ILogger logger)
    {
        _logger = logger;
    }

    protected void LogUseCaseExecutionStarted(object useCaseInput)
    {
        var useCaseName = GetType().Name.Replace("UseCase", string.Empty);
        _logger.LogInformation(
            UseCaseExecutionStartedLogMessage,
            useCaseName,
            useCaseInput);
    }
}
