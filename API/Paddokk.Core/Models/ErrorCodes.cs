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
    public const string LikeOwnSubject = "LIKE_OWN_SUBJECT";

    // Username change (see ChangeUsernameHandler) — each maps to inline, field-level copy.
    public const string UsernameTaken = "USERNAME_TAKEN";
    public const string UsernameReserved = "USERNAME_RESERVED";
    public const string UsernameChangeTooSoon = "USERNAME_CHANGE_TOO_SOON";

    // Data export (see RequestDataExportHandler).
    public const string ExportCooldown = "EXPORT_COOLDOWN";

    // Image-upload validator codes (see IImageUploadValidator).
    public const string UploadRequired = "UPLOAD_REQUIRED";
    public const string UploadTooLarge = "UPLOAD_TOO_LARGE";
    public const string UploadUnsupportedFormat = "UPLOAD_UNSUPPORTED_FORMAT";
    public const string UploadContentMismatch = "UPLOAD_CONTENT_MISMATCH";
    public const string UploadDimensionsTooSmall = "UPLOAD_DIMENSIONS_TOO_SMALL";
    public const string UploadDimensionsTooLarge = "UPLOAD_DIMENSIONS_TOO_LARGE";
    public const string UploadInvalidImage = "UPLOAD_INVALID_IMAGE";

    private static readonly HashSet<string> KnownCodes = new(StringComparer.Ordinal)
    {
        NotFound, Conflict, Forbidden, ValidationFailed, Internal, RateLimited, RequestCancelled,
        SubscribeToOwnSubject, LikeOwnSubject,
        UsernameTaken, UsernameReserved, UsernameChangeTooSoon, ExportCooldown,
        UploadRequired, UploadTooLarge, UploadUnsupportedFormat, UploadContentMismatch,
        UploadDimensionsTooSmall, UploadDimensionsTooLarge, UploadInvalidImage
    };

    /// <summary>
    /// True when <paramref name="code"/> is one of our stable contract codes. Used to keep
    /// FluentValidation's internal validator names (e.g. "NotEmptyValidator") out of the
    /// API contract by normalizing unknown codes to <see cref="ValidationFailed"/>.
    /// </summary>
    public static bool IsKnown(string? code) => code is not null && KnownCodes.Contains(code);
}
