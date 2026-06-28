using Microsoft.AspNetCore.Http;
using NSubstitute;
using SkiaSharp;

namespace Paddokk.Tests.Common;

internal static class ImageUploadTestFiles
{
    internal static readonly byte[] JpegMagic = [0xFF, 0xD8, 0xFF, 0xE0];

    internal static readonly byte[] PngMagic =
        [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];

    // RIFF????WEBP — file size bytes can be anything for magic-byte purposes.
    internal static readonly byte[] WebpMagic =
        [0x52, 0x49, 0x46, 0x46, 0x00, 0x00, 0x00, 0x00, 0x57, 0x45, 0x42, 0x50];

    internal static IFormFile MakeFile(byte[] payload, string contentType, long? lengthOverride = null)
    {
        var file = Substitute.For<IFormFile>();
        file.Length.Returns(lengthOverride ?? payload.LongLength);
        file.ContentType.Returns(contentType);
        file.FileName.Returns("test.bin");
        file.OpenReadStream().Returns(_ => new MemoryStream(payload, writable: false));
        return file;
    }

    // The happy-path fixtures are real, decodable images at a valid size so they pass
    // the validator's decode/dimension gate. `sizeBytes` overrides only the reported
    // Length (for size-boundary tests); the actual bytes remain a valid image.
    internal static IFormFile JpegFile(int? sizeBytes = null) =>
        MakeFile(EncodeImage(SKEncodedImageFormat.Jpeg, 200, 200), "image/jpeg", sizeBytes);

    internal static IFormFile PngFile(int? sizeBytes = null) =>
        MakeFile(EncodeImage(SKEncodedImageFormat.Png, 200, 200), "image/png", sizeBytes);

    internal static IFormFile WebpFile(int? sizeBytes = null) =>
        MakeFile(EncodeImage(SKEncodedImageFormat.Webp, 200, 200), "image/webp", sizeBytes);

    /// <summary>A real JPEG of the given dimensions, for exercising the dimension checks.</summary>
    internal static IFormFile JpegFileOfSize(int width, int height) =>
        MakeFile(EncodeImage(SKEncodedImageFormat.Jpeg, width, height), "image/jpeg");

    /// <summary>Real magic bytes for the content type, but a body Skia cannot decode.</summary>
    internal static IFormFile UndecodableImage(byte[] magic, string contentType)
    {
        var payload = new byte[magic.Length + 16];
        Array.Copy(magic, payload, magic.Length);
        return MakeFile(payload, contentType);
    }

    internal static byte[] EncodeImage(SKEncodedImageFormat format, int width, int height)
    {
        using var surface = SKSurface.Create(new SKImageInfo(width, height));
        surface.Canvas.Clear(SKColors.CornflowerBlue);
        using var image = surface.Snapshot();
        using var data = image.Encode(format, 90);
        return data.ToArray();
    }
}