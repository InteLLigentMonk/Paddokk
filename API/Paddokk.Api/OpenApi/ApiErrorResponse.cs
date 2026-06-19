namespace Paddokk.Api.OpenApi;

/// <summary>
/// Canonical error envelope returned by the API for every failure, whether it
/// originates from a domain <c>Result</c> (via <c>ApiControllerBase.FromError</c>) or
/// an unhandled exception (via <c>GlobalExceptionMiddleware</c>). The frontend branches
/// on <see cref="Code"/> to render specific, actionable messages.
/// </summary>
public sealed record ApiErrorResponse(
    string Code,
    string Message,
    int Status,
    IReadOnlyList<ApiValidationError>? Errors = null);

/// <summary>A single field-level validation failure within an <see cref="ApiErrorResponse"/>.</summary>
public sealed record ApiValidationError(string Field, string Code, string Message);
