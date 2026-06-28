using System.Net;
using System.Text.Json;
using EmployeeApi.Application.Common.Exceptions;
using FluentValidation;

namespace EmployeeApi.Api.Middleware;

/// <summary>
/// Single place that turns exceptions into HTTP responses, so controllers and
/// handlers never have to think about status codes — they just throw.
/// </summary>
public sealed class ExceptionHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger)
    {
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            await WriteResponseAsync(context, HttpStatusCode.BadRequest, new
            {
                title = "Validation failed",
                errors = ex.Errors.Select(e => e.ErrorMessage)
            });
        }
        catch (NotFoundException ex)
        {
            await WriteResponseAsync(context, HttpStatusCode.NotFound, new
            {
                title = ex.Message
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception occurred");
            await WriteResponseAsync(context, HttpStatusCode.InternalServerError, new
            {
                title = "An unexpected error occurred."
            });
        }
    }

    private static Task WriteResponseAsync(HttpContext context, HttpStatusCode statusCode, object payload)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;
        return context.Response.WriteAsync(JsonSerializer.Serialize(payload));
    }
}
