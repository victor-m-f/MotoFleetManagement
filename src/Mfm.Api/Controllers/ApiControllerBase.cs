using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mfm.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase<TApiController> : ControllerBase
    where TApiController : ApiControllerBase<TApiController>
{
    private const string LogResponseTemplate = "Response {@apiResponse} returned.";

    private readonly ILogger _logger;
    protected IMediator Mediator { get; }

    protected ApiControllerBase(ILogger<TApiController> logger, IMediator mediator)
    {
        _logger = logger;
        Mediator = mediator;
    }

    protected IDisposable? StartUseCaseScope(string useCaseName) =>
        _logger.BeginScope(new Dictionary<string, string> { { "UseCase", useCaseName } });

    protected IActionResult Respond(string routeName, object routeValues, object value)
    {
        LogResponse(value);
        return CreatedAtRoute(routeName, routeValues, value);
    }

    private void LogResponse(object response)
    {
        _logger.LogInformation(LogResponseTemplate, response);
    }
}

