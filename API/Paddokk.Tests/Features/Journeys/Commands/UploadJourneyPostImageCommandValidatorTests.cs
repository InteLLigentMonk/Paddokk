using FluentAssertions;
using Paddokk.Core.Common.ImageUpload;
using Paddokk.Core.Features.Journeys.Commands.UploadJourneyPostImage;
using Paddokk.Tests.Common;

namespace Paddokk.Tests.Features.Journeys.Commands;

public class UploadJourneyPostImageCommandValidatorTests
{
    private readonly UploadJourneyPostImageCommandValidator _validator = new(new ImageUploadValidator());

    [Fact]
    public async Task Validate_ValidJpeg_Passes()
    {
        var command = new UploadJourneyPostImageCommand(1, ImageUploadTestFiles.JpegFile());

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ValidPng_Passes()
    {
        var command = new UploadJourneyPostImageCommand(1, ImageUploadTestFiles.PngFile());

        var result = await _validator.ValidateAsync(command);

        result.IsValid.Should().BeTrue();
    }

    [Fact]
    public async Task Validate_ValidWebp_Passes()
    {
        var command = new UploadJourneyPostImageCommand(1, ImageUploadTestFiles.WebpFile());

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

        var result = await _validator.ValidateAsync(new UploadJourneyPostImageCommand(1, file));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_DisallowedContentType_Fails()
    {
        var file = ImageUploadTestFiles.MakeFile(ImageUploadTestFiles.JpegMagic, "application/pdf");

        var result = await _validator.ValidateAsync(new UploadJourneyPostImageCommand(1, file));

        result.IsValid.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_MimeMagicMismatch_Fails()
    {
        var file = ImageUploadTestFiles.MakeFile(ImageUploadTestFiles.PngMagic, "image/jpeg");

        var result = await _validator.ValidateAsync(new UploadJourneyPostImageCommand(1, file));

        result.IsValid.Should().BeFalse();
    }
}
