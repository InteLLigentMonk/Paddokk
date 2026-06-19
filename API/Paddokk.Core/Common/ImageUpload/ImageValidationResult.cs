namespace Paddokk.Core.Common.ImageUpload;

public sealed record ImageValidationResult(bool IsValid, string? Reason, string? Code = null)
{
    public static ImageValidationResult Valid() => new(true, null);
    public static ImageValidationResult Invalid(string reason, string code) => new(false, reason, code);
}
