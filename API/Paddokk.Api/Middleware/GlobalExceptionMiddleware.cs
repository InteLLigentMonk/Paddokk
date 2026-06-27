using System.Text.Json;
using FluentValidation;
using Paddokk.Api.OpenApi;
using Paddokk.Core.Models;

namespace Paddokk.Api.Middleware;

public class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

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
            await WriteErrorResponseAsync(context, ex);
        }
    }

    private async Task WriteErrorResponseAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        // Client-facing Message is diagnostic only and never rendered (see ADR-0007), so the
        // typed branches return fixed text rather than ex.Message: no internal detail can leak.
        var response = ex switch
        {
            ValidationException ve => BuildValidationResponse(ve),
            UnauthorizedAccessException => new ApiErrorResponse(
                ErrorCodes.Forbidden, "Access denied", StatusCodes.Status403Forbidden),
            KeyNotFoundException => new ApiErrorResponse(
                ErrorCodes.NotFound, "Resource not found", StatusCodes.Status404NotFound),
            ArgumentException or InvalidOperationException => new ApiErrorResponse(
                ErrorCodes.ValidationFailed, "Invalid request", StatusCodes.Status400BadRequest),
            OperationCanceledException => new ApiErrorResponse(
                ErrorCodes.RequestCancelled, "Request was cancelled", StatusCodes.Status503ServiceUnavailable),
            _ => new ApiErrorResponse(
                ErrorCodes.Internal, "An unexpected error occurred", StatusCodes.Status500InternalServerError)
        } with { TraceId = context.TraceIdentifier };

        // 5xx are defects worth paging on; expected 4xx (validation, not-found, conflicts) are
        // normal traffic and log at Information so they don't drown the error stream.
        if (response.Status >= StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(ex, "Unhandled exception on {Method} {Path} ({TraceId})",
                context.Request.Method, context.Request.Path, context.TraceIdentifier);
        }
        else
        {
            _logger.LogInformation(ex, "Handled {Status} on {Method} {Path} ({TraceId})",
                response.Status, context.Request.Method, context.Request.Path, context.TraceIdentifier);
        }

        context.Response.StatusCode = response.Status;
        await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
    }

    private static ApiErrorResponse BuildValidationResponse(ValidationException ve)
    {
        var errors = ve.Errors
            .Select(failure => new ApiValidationError(
                failure.PropertyName,
                ErrorCodes.IsKnown(failure.ErrorCode) ? failure.ErrorCode : ErrorCodes.ValidationFailed,
                failure.ErrorMessage))
            .ToList();

        var message = string.Join("; ", errors.Select(e => e.Message));

        // When a single failure carries a stable code (e.g. an upload rejection), surface it
        // at the top level so the frontend can branch without inspecting the errors array.
        var topCode = errors.Count == 1 && ErrorCodes.IsKnown(errors[0].Code)
            ? errors[0].Code
            : ErrorCodes.ValidationFailed;

        return new ApiErrorResponse(topCode, message, StatusCodes.Status400BadRequest, errors);
    }
}
