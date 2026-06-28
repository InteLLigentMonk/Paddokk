using Microsoft.AspNetCore.Http;
using Paddokk.Core.Models;
using SkiaSharp;

namespace Paddokk.Core.Common.ImageUpload;

public sealed class ImageUploadValidator : IImageUploadValidator
{
    public const long MaxFileSizeBytes = 5 * 1024 * 1024;

    // Below this an image is too small to be useful; above it the master image
    // (resized for edge optimization) would waste storage and decode time.
    public const int MinDimensionPixels = 100;
    public const int MaxDimensionPixels = 4000;

    public static readonly IReadOnlyList<string> AllowedContentTypes =
        new[] { "image/jpeg", "image/png", "image/webp" };

    public ImageValidationResult Validate(IFormFile? file)
    {
        if (file is null || file.Length == 0)
            return ImageValidationResult.Invalid("File is required", ErrorCodes.UploadRequired);

        if (file.Length > MaxFileSizeBytes)
            return ImageValidationResult.Invalid(
                $"File exceeds maximum size of {MaxFileSizeBytes / (1024 * 1024)} MiB",
                ErrorCodes.UploadTooLarge);

        var contentType = file.ContentType?.ToLowerInvariant();
        if (contentType is null || !AllowedContentTypes.Contains(contentType))
            return ImageValidationResult.Invalid(
                $"Content type '{file.ContentType}' is not allowed. Allowed types: {string.Join(", ", AllowedContentTypes)}",
                ErrorCodes.UploadUnsupportedFormat);

        if (!MagicBytesMatchContentType(file, contentType))
            return ImageValidationResult.Invalid(
                $"File contents do not match declared content type '{contentType}'",
                ErrorCodes.UploadContentMismatch);

        return ValidateDecodableDimensions(file);
    }

    // The single gate for "is this actually a usable image": decodes the header to
    // confirm the bytes form a real image and that its dimensions are in range.
    // Header-only (SKCodec.Info), so it does not pay the cost of a full pixel decode.
    private static ImageValidationResult ValidateDecodableDimensions(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        using var codec = SKCodec.Create(stream);
        if (codec is null)
            return ImageValidationResult.Invalid(
                "File could not be decoded as an image", ErrorCodes.UploadInvalidImage);

        var info = codec.Info;

        if (info.Width < MinDimensionPixels || info.Height < MinDimensionPixels)
            return ImageValidationResult.Invalid(
                $"Image is {info.Width}x{info.Height}px; minimum is {MinDimensionPixels}x{MinDimensionPixels}px",
                ErrorCodes.UploadDimensionsTooSmall);

        if (info.Width > MaxDimensionPixels || info.Height > MaxDimensionPixels)
            return ImageValidationResult.Invalid(
                $"Image is {info.Width}x{info.Height}px; maximum is {MaxDimensionPixels}x{MaxDimensionPixels}px",
                ErrorCodes.UploadDimensionsTooLarge);

        return ImageValidationResult.Valid();
    }

    private static bool MagicBytesMatchContentType(IFormFile file, string contentType)
    {
        var required = contentType switch
        {
            "image/jpeg" => 3,
            "image/png" => 8,
            "image/webp" => 12,
            _ => 0
        };

        if (required == 0 || file.Length < required)
            return false;

        Span<byte> header = stackalloc byte[12];
        using var stream = file.OpenReadStream();
        var totalRead = 0;
        while (totalRead < required)
        {
            var read = stream.Read(header[totalRead..required]);
            if (read == 0) return false;
            totalRead += read;
        }

        return contentType switch
        {
            "image/jpeg" =>
                header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,

            "image/png" =>
                header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47 &&
                header[4] == 0x0D && header[5] == 0x0A && header[6] == 0x1A && header[7] == 0x0A,

            "image/webp" =>
                header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 &&
                header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50,

            _ => false
        };
    }
}
