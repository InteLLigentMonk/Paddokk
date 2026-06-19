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
            _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                context.Request.Method, context.Request.Path);

            await WriteErrorResponseAsync(context, ex);
        }
    }

    private static async Task WriteErrorResponseAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";

        var response = ex switch
        {
            ValidationException ve => BuildValidationResponse(ve),
            UnauthorizedAccessException => new ApiErrorResponse(
                ErrorCodes.Forbidden, ex.Message, StatusCodes.Status403Forbidden),
            KeyNotFoundException => new ApiErrorResponse(
                ErrorCodes.NotFound, ex.Message, StatusCodes.Status404NotFound),
            ArgumentException => new ApiErrorResponse(
                ErrorCodes.ValidationFailed, ex.Message, StatusCodes.Status400BadRequest),
            InvalidOperationException => new ApiErrorResponse(
                ErrorCodes.ValidationFailed, ex.Message, StatusCodes.Status400BadRequest),
            OperationCanceledException => new ApiErrorResponse(
                ErrorCodes.RequestCancelled, "Request was cancelled", StatusCodes.Status503ServiceUnavailable),
            // Do not leak internal exception details to clients.
            _ => new ApiErrorResponse(
                ErrorCodes.Internal, "An unexpected error occurred", StatusCodes.Status500InternalServerError)
        };

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
