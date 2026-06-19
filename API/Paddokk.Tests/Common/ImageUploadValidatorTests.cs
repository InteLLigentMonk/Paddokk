using FluentAssertions;
using Paddokk.Core.Common.ImageUpload;
using Paddokk.Core.Models;

namespace Paddokk.Tests.Common;

public class ImageUploadValidatorTests
{
    private readonly ImageUploadValidator _validator = new();

    [Fact]
    public void Validate_JpegHappyPath_ReturnsValid()
    {
        var file = ImageUploadTestFiles.JpegFile();

        var result = _validator.Validate(file);

        result.IsValid.Should().BeTrue();
        result.Reason.Should().BeNull();
    }

    [Fact]
    public void Validate_PngHappyPath_ReturnsValid()
    {
        var file = ImageUploadTestFiles.PngFile();

        var result = _validator.Validate(file);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_WebpHappyPath_ReturnsValid()
    {
        var file = ImageUploadTestFiles.WebpFile();

        var result = _validator.Validate(file);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_ExactlyMaxSize_ReturnsValid()
    {
        var file = ImageUploadTestFiles.JpegFile(sizeBytes: (int)ImageUploadValidator.MaxFileSizeBytes);

        var result = _validator.Validate(file);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public void Validate_OverMaxSize_ReturnsInvalidWithSizeReason()
    {
        var file = ImageUploadTestFiles.MakeFile(
            ImageUploadTestFiles.JpegMagic,
            "image/jpeg",
            lengthOverride: ImageUploadValidator.MaxFileSizeBytes + 1);

        var result = _validator.Validate(file);

        result.IsValid.Should().BeFalse();
        result.Reason.Should().Contain("5");
        result.Reason.Should().ContainAny("MiB", "size", "large");
    }

    [Fact]
    public void Validate_DisallowedContentType_ReturnsInvalidWithTypeReason()
    {
        var file = ImageUploadTestFiles.MakeFile(
            ImageUploadTestFiles.JpegMagic,
            "application/pdf");

        var result = _validator.Validate(file);

        result.IsValid.Should().BeFalse();
        result.Reason.Should().Contain("application/pdf");
    }

    [Fact]
    public void Validate_JpegContentTypeButPngBytes_ReturnsInvalidWithMismatchReason()
    {
        var file = ImageUploadTestFiles.MakeFile(
            ImageUploadTestFiles.PngMagic,
            "image/jpeg");

        var result = _validator.Validate(file);

        result.IsValid.Should().BeFalse();
        result.Reason.Should().ContainAny("match", "magic", "contents");
    }

    [Fact]
    public void Validate_PngContentTypeButJpegBytes_ReturnsInvalidWithMismatchReason()
    {
        var file = ImageUploadTestFiles.MakeFile(
            ImageUploadTestFiles.JpegMagic,
            "image/png");

        var result = _validator.Validate(file);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_WebpContentTypeButJpegBytes_ReturnsInvalid()
    {
        var file = ImageUploadTestFiles.MakeFile(
            ImageUploadTestFiles.JpegMagic,
            "image/webp");

        var result = _validator.Validate(file);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_ZeroByteFile_ReturnsInvalid()
    {
        var file = ImageUploadTestFiles.MakeFile([], "image/jpeg");

        var result = _validator.Validate(file);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_NullFile_ReturnsInvalid()
    {
        var result = _validator.Validate(null);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_TruncatedPngHeader_ReturnsInvalid()
    {
        // PNG signature is 8 bytes; provide only 4.
        var file = ImageUploadTestFiles.MakeFile([0x89, 0x50, 0x4E, 0x47], "image/png");

        var result = _validator.Validate(file);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_TruncatedJpegHeader_ReturnsInvalid()
    {
        // JPEG signature needs 3 bytes; provide only 2.
        var file = ImageUploadTestFiles.MakeFile([0xFF, 0xD8], "image/jpeg");

        var result = _validator.Validate(file);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_TruncatedWebpHeader_ReturnsInvalid()
    {
        // WebP signature needs 12 bytes; provide only 4 (RIFF).
        var file = ImageUploadTestFiles.MakeFile([0x52, 0x49, 0x46, 0x46], "image/webp");

        var result = _validator.Validate(file);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Validate_OverMaxSize_HasUploadTooLargeCode()
    {
        var file = ImageUploadTestFiles.MakeFile(
            ImageUploadTestFiles.JpegMagic,
            "image/jpeg",
            lengthOverride: ImageUploadValidator.MaxFileSizeBytes + 1);

        _validator.Validate(file).Code.Should().Be(ErrorCodes.UploadTooLarge);
    }

    [Fact]
    public void Validate_DisallowedContentType_HasUnsupportedFormatCode()
    {
        var file = ImageUploadTestFiles.MakeFile(ImageUploadTestFiles.JpegMagic, "application/pdf");

        _validator.Validate(file).Code.Should().Be(ErrorCodes.UploadUnsupportedFormat);
    }

    [Fact]
    public void Validate_MagicByteMismatch_HasContentMismatchCode()
    {
        var file = ImageUploadTestFiles.MakeFile(ImageUploadTestFiles.PngMagic, "image/jpeg");

        _validator.Validate(file).Code.Should().Be(ErrorCodes.UploadContentMismatch);
    }

    [Fact]
    public void Validate_NullFile_HasUploadRequiredCode()
    {
        _validator.Validate(null).Code.Should().Be(ErrorCodes.UploadRequired);
    }

    [Fact]
    public void Validate_HappyPath_HasNullCode()
    {
        _validator.Validate(ImageUploadTestFiles.JpegFile()).Code.Should().BeNull();
    }
}