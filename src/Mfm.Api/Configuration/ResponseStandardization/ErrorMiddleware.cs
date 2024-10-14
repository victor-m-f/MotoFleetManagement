using Mfm.Application.Dtos.Common;
using Mfm.Domain.Exceptions;
using System.Text.Json;

namespace Mfm.Api.Configuration.ResponseStandardization;

public class ErrorMiddleware
{
    private const string LogResponseTemplate = "Response {@apiResponse} returned.";
    private const string DefaultInternalErrorMessage = "An unexpected error occurred. Please try again later or contact support if the problem persists.";

    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ErrorMiddleware(
        RequestDelegate next,
        ILogger<ErrorMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (ValidationException ex)
        {
            await HandleExceptionAsync(httpContext, ex, StatusCodes.Status400BadRequest);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex, StatusCodes.Status500InternalServerError);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception, int httpStatusCode)
    {
        var errors = new List<string> { DefaultInternalErrorMessage };

        if (!_environment.IsProduction())
        {
            errors.Clear();

            var error = exception;
            while (error != null)
            {
                errors.Add(error.Message);
                error = error.InnerException;
            }
        }

        var apiResponse = new MessageDto
        {
            Message = string.Join(';', errors.Select(x => x))
        };
        _logger.LogError(LogResponseTemplate, apiResponse);
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = httpStatusCode;
        return context.Response.WriteAsync(JsonSerializer.Serialize(apiResponse));
    }
}
