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
    public static readonly Error None = new(ErrorType.None, string.Empty);

    public static Error NotFound(string message) => new(ErrorType.NotFound, message);
    public static Error Unauthorized(string message) => new(ErrorType.Unauthorized, message);
    public static Error Conflict(string message) => new(ErrorType.Conflict, message);
    public static Error Validation(string message) => new(ErrorType.Validation, message);
    public static Error Internal(string message) => new(ErrorType.Internal, message);
}
