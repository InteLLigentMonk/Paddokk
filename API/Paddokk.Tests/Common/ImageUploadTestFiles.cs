using Microsoft.AspNetCore.Http;
using NSubstitute;

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

    internal static IFormFile JpegFile(int sizeBytes = 1024) =>
        MakeFile(BuildPayload(JpegMagic, sizeBytes), "image/jpeg");

    internal static IFormFile PngFile(int sizeBytes = 1024) =>
        MakeFile(BuildPayload(PngMagic, sizeBytes), "image/png");

    internal static IFormFile WebpFile(int sizeBytes = 1024) =>
        MakeFile(BuildPayload(WebpMagic, sizeBytes), "image/webp");

    internal static byte[] BuildPayload(byte[] magic, int totalSizeBytes)
    {
        if (totalSizeBytes < magic.Length)
            return magic.AsSpan(0, totalSizeBytes).ToArray();

        var payload = new byte[totalSizeBytes];
        Array.Copy(magic, payload, magic.Length);
        return payload;
    }
}