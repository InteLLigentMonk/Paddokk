namespace Paddokk.Core.Models;

/// <summary>
/// Stable, machine-readable error codes returned to the client in the API error
/// envelope. Values are part of the API contract: the frontend branches on them
/// to render specific, actionable messages, so they must not change once shipped.
/// </summary>
public static class ErrorCodes
{
    // Generic codes derived from <see cref="ErrorType"/>.
    public const string NotFound = "NOT_FOUND";
    public const string Conflict = "CONFLICT";
    public const string Forbidden = "FORBIDDEN";
    public const string ValidationFailed = "VALIDATION_FAILED";
    public const string Internal = "INTERNAL";
    public const string RateLimited = "RATE_LIMITED";
    public const string RequestCancelled = "REQUEST_CANCELLED";

    // Domain-specific codes.
    public const string SubscribeToOwnSubject = "SUBSCRIBE_TO_OWN_SUBJECT";

    // Image-upload validator codes (see IImageUploadValidator).
    public const string UploadRequired = "UPLOAD_REQUIRED";
    public const string UploadTooLarge = "UPLOAD_TOO_LARGE";
    public const string UploadUnsupportedFormat = "UPLOAD_UNSUPPORTED_FORMAT";
    public const string UploadContentMismatch = "UPLOAD_CONTENT_MISMATCH";

    private static readonly HashSet<string> KnownCodes = new(StringComparer.Ordinal)
    {
        NotFound, Conflict, Forbidden, ValidationFailed, Internal, RateLimited, RequestCancelled,
        SubscribeToOwnSubject,
        UploadRequired, UploadTooLarge, UploadUnsupportedFormat, UploadContentMismatch
    };

    /// <summary>
    /// True when <paramref name="code"/> is one of our stable contract codes. Used to keep
    /// FluentValidation's internal validator names (e.g. "NotEmptyValidator") out of the
    /// API contract by normalizing unknown codes to <see cref="ValidationFailed"/>.
    /// </summary>
    public static bool IsKnown(string? code) => code is not null && KnownCodes.Contains(code);
}
