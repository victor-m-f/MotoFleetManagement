using FluentAssertions;
using Mfm.Api.Configuration.ResponseStandardization;
using Mfm.Application.Dtos.Common;
using Mfm.Domain.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Text.Json;

namespace Mfm.Api.UnitTests.Configuration.ResponseStandardization;
public sealed class ErrorMiddlewareTests
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly ErrorMiddleware _middleware;
    private readonly DefaultHttpContext _httpContext;

    public ErrorMiddlewareTests()
    {
        _next = Substitute.For<RequestDelegate>();
        _logger = Substitute.For<ILogger<ErrorMiddleware>>();
        _environment = Substitute.For<IWebHostEnvironment>();
        _httpContext = new DefaultHttpContext();
        _middleware = new ErrorMiddleware(_next, _logger, _environment);
    }

    [Fact]
    public async Task InvokeAsync_ShouldCallNext_WhenNoExceptionIsThrown()
    {
        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        await _next.Received(1).Invoke(_httpContext);
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnBadRequest_WhenValidationExceptionIsThrown()
    {
        // Arrange
        var memoryStream = new MemoryStream();
        _httpContext.Response.Body = memoryStream;

        _next.When(x => x.Invoke(Arg.Any<HttpContext>()))
             .Do(_ => throw new ValidationException());

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);

        memoryStream.Seek(0, SeekOrigin.Begin);
        var responseContent = await new StreamReader(memoryStream).ReadToEndAsync();

        var messageDto = JsonSerializer.Deserialize<MessageDto>(responseContent);
        messageDto!.Message.Should().Be("Dados inválidos");

        _logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("Dados inválidos")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task InvokeAsync_ShouldReturnInternalServerError_WhenGenericExceptionIsThrown()
    {
        // Arrange
        var memoryStream = new MemoryStream();
        _httpContext.Response.Body = memoryStream;

        _next
            .When(x => x.Invoke(Arg.Any<HttpContext>()))
            .Do(_ => throw new Exception("Internal server error"));

        _environment.EnvironmentName.Returns("Production");

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        memoryStream.Seek(0, SeekOrigin.Begin);
        var responseContent = await new StreamReader(memoryStream).ReadToEndAsync();
        responseContent.Should().Contain("An unexpected error occurred");

        _logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("An unexpected error occurred")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    public async Task InvokeAsync_ShouldIncludeExceptionDetails_WhenNotInProduction()
    {
        // Arrange
        var memoryStream = new MemoryStream();
        _httpContext.Response.Body = memoryStream;

        _next
            .When(x => x.Invoke(Arg.Any<HttpContext>()))
            .Do(_ => throw new Exception("Detailed error"));

        _environment.EnvironmentName.Returns("Development");

        // Act
        await _middleware.InvokeAsync(_httpContext);

        // Assert
        _httpContext.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        memoryStream.Seek(0, SeekOrigin.Begin);
        var responseContent = await new StreamReader(memoryStream).ReadToEndAsync();
        responseContent.Should().Contain("Detailed error");

        _logger.Received(1).Log(
            LogLevel.Error,
            Arg.Any<EventId>(),
            Arg.Is<object>(o => o.ToString()!.Contains("Detailed error")),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}
