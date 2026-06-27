namespace Paddokk.Api.OpenApi;

/// <summary>
/// Canonical error envelope returned by the API for every failure, whether it
/// originates from a domain <c>Result</c> (via <c>ApiControllerBase.FromError</c>) or
/// an unhandled exception (via <c>GlobalExceptionMiddleware</c>). The frontend branches
/// on <see cref="Code"/> to render specific, actionable messages; <see cref="Message"/>
/// is diagnostic (logs/developers) and is never rendered to users.
/// </summary>
/// <param name="TraceId">
/// Per-request correlation id (the request's <c>TraceIdentifier</c>), echoed so a user's
/// "Report a problem" can be matched to the corresponding server log line.
/// </param>
public sealed record ApiErrorResponse(
    string Code,
    string Message,
    int Status,
    IReadOnlyList<ApiValidationError>? Errors = null,
    string? TraceId = null);

/// <summary>A single field-level validation failure within an <see cref="ApiErrorResponse"/>.</summary>
public sealed record ApiValidationError(string Field, string Code, string Message);
