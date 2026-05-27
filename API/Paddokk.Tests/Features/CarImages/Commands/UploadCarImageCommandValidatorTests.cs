using FluentAssertions;
using Paddokk.Core.Common.ImageUpload;
using Paddokk.Core.Features.CarImages.Commands.UploadCarImage;
using Paddokk.Tests.Common;

namespace Paddokk.Tests.Features.CarImages.Commands;

public class UploadCarImageCommandValidatorTests
{
    private readonly UploadCarImageCommandValidator _validator = new(new ImageUploadValidator());

    [Fact]
    public async Task Validate_ValidJpeg_Passes()
    {
        var command = new UploadCarImageCommand(1, ImageUploadTestFiles.JpegFile(), Caption: null);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ValidPng_Passes()
    {
        var command = new UploadCarImageCommand(1, ImageUploadTestFiles.PngFile(), Caption: null);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ValidWebp_Passes()
    {
        var command = new UploadCarImageCommand(1, ImageUploadTestFiles.WebpFile(), Caption: null);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_OversizeFile_Fails()
    {
        var file = ImageUploadTestFiles.MakeFile(
            ImageUploadTestFiles.JpegMagic,
            "image/jpeg",
            lengthOverride: ImageUploadValidator.MaxFileSizeBytes + 1);
        var command = new UploadCarImageCommand(1, file, Caption: null);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_DisallowedContentType_Fails()
    {
        var file = ImageUploadTestFiles.MakeFile(ImageUploadTestFiles.JpegMagic, "application/pdf");
        var command = new UploadCarImageCommand(1, file, Caption: null);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_MimeMagicMismatch_Fails()
    {
        var file = ImageUploadTestFiles.MakeFile(ImageUploadTestFiles.PngMagic, "image/jpeg");
        var command = new UploadCarImageCommand(1, file, Caption: null);

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeFalse();
    }
}