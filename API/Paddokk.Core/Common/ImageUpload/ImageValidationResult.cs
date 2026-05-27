namespace Paddokk.Core.Common.ImageUpload;

public sealed record ImageValidationResult(bool IsValid, string? Reason)
{
    public static ImageValidationResult Valid() => new(true, null);
    public static ImageValidationResult Invalid(string reason) => new(false, reason);
}
