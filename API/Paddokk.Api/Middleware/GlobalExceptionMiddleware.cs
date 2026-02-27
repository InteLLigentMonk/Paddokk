using System.Net;
using System.Text.Json;
using FluentValidation;

namespace Paddokk.Api.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await WriteErrorResponseAsync(context, ex);
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var (statusCode, message) = ex switch
        {
            ValidationException ve => (HttpStatusCode.BadRequest,
                string.Join("; ", ve.Errors.Select(e => e.ErrorMessage))),
            UnauthorizedAccessException => (HttpStatusCode.Forbidden, ex.Message),
            KeyNotFoundException => (HttpStatusCode.NotFound, ex.Message),
            ArgumentException => (HttpStatusCode.BadRequest, ex.Message),
            InvalidOperationException => (HttpStatusCode.BadRequest, ex.Message),
            OperationCanceledException => (HttpStatusCode.ServiceUnavailable, "Request was cancelled"),
            _ => (HttpStatusCode.InternalServerError, "An unexpected error occurred")
        };

        context.Response.StatusCode = (int)statusCode;

        var response = new
        {
            error = message,
            statusCode = (int)statusCode,
            path = context.Request.Path.Value
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}