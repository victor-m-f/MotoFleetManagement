using MediatR;
using Mfm.Application.UseCases.Base;
using Microsoft.AspNetCore.Mvc;

namespace Mfm.Api.Controllers;

[ApiController]
public abstract class ApiControllerBase<TApiController> : ControllerBase
    where TApiController : ApiControllerBase<TApiController>
{
    private const string LogResponseTemplate = "Response with StatusCode {StatusCode}: {@ApiResponse}";

    private readonly ILogger _logger;
    protected IMediator Mediator { get; }

    protected ApiControllerBase(ILogger<TApiController> logger, IMediator mediator)
    {
        _logger = logger;
        Mediator = mediator;
    }

    protected IDisposable? StartUseCaseScope(string useCaseName) =>
        _logger.BeginScope(new Dictionary<string, string> { { "UseCase", useCaseName } });

    protected IActionResult RespondError(OutputBase output)
    {
        var response = new
        {
            output.Errors
        };

        LogResponse(output.StatusCode, response);

        return StatusCode(output.StatusCode, response);
    }

    protected IActionResult RespondCreated(
        string routeName,
        object routeValues,
        object value)
    {
        LogResponse(StatusCodes.Status201Created, value);
        return CreatedAtRoute(routeName, routeValues, value);
    }

    private void LogResponse(int statusCode, object response)
    {
        _logger.LogInformation(LogResponseTemplate, statusCode, response);
    }
}
