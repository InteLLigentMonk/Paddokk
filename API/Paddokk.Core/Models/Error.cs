namespace Paddokk.Core.Models;

public enum ErrorType
{
    None,
    NotFound,
    Unauthorized,
    Conflict,
    Validation,
    Internal
}

public record Error(ErrorType Type, string Message)
{
    /// <summary>
    /// Stable, machine-readable code from <see cref="ErrorCodes"/>. Defaults to the code
    /// for the <see cref="ErrorType"/>; pass an explicit code for domain-specific cases.
    /// </summary>
    public string Code { get; init; } = string.Empty;

    public static readonly Error None = new(ErrorType.None, string.Empty);

    public static Error NotFound(string message, string? code = null) =>
        new(ErrorType.NotFound, message) { Code = code ?? ErrorCodes.NotFound };

    public static Error Unauthorized(string message, string? code = null) =>
        new(ErrorType.Unauthorized, message) { Code = code ?? ErrorCodes.Forbidden };

    public static Error Conflict(string message, string? code = null) =>
        new(ErrorType.Conflict, message) { Code = code ?? ErrorCodes.Conflict };

    public static Error Validation(string message, string? code = null) =>
        new(ErrorType.Validation, message) { Code = code ?? ErrorCodes.ValidationFailed };

    public static Error Internal(string message, string? code = null) =>
        new(ErrorType.Internal, message) { Code = code ?? ErrorCodes.Internal };
}
