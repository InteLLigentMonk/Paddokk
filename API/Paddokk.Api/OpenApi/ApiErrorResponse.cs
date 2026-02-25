namespace Paddokk.Api.OpenApi;

/// <summary>
/// Matches the JSON shape returned by GlobalExceptionMiddleware.
/// </summary>
public class ApiErrorResponse
{
    /// <summary>Human-readable error message.</summary>
    public string Error { get; set; } = string.Empty;

    /// <summary>HTTP status code.</summary>
    public int StatusCode { get; set; }

    /// <summary>Request path that triggered the error.</summary>
    public string Path { get; set; } = string.Empty;
}
