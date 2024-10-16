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

    protected IActionResult Respond(OutputBase output, object? value = null)
    {
        if (output.IsInvalid)
        {
            return RespondError(output);
        }

        if (output.StatusCode == StatusCodes.Status201Created)
        {
            _logger.LogWarning(
                "Status 201 detected. Prefer using the overload with route name and values for CreatedAtRoute responses.");
        }

        if (value is not null)
        {
            LogResponse(output.StatusCode, value);
        }

        return StatusCode(output.StatusCode, value);
    }

    protected IActionResult Respond(
        OutputBase output,
        string routeName,
        object routeValues,
        object value)
    {
        if (output.IsInvalid)
        {
            return RespondError(output);
        }

        LogResponse(StatusCodes.Status201Created, value);
        return CreatedAtRoute(routeName, routeValues, value);
    }

    private ObjectResult RespondError(OutputBase output)
    {
        var response = new
        {
            output.Errors
        };

        LogResponse(output.StatusCode, response);

        return StatusCode(output.StatusCode, response);
    }

    private void LogResponse(int statusCode, object response)
    {
        _logger.LogInformation(LogResponseTemplate, statusCode, response);
    }
}
