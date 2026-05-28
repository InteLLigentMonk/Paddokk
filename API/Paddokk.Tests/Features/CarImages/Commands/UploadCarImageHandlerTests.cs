using FluentAssertions;
using Microsoft.AspNetCore.Http;
using NSubstitute;
using Paddokk.Core.Features.CarImages.Commands.UploadCarImage;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Image;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.CarImages.Commands;

public class UploadCarImageHandlerTests
{
    private const int CarId = 1;
    private const string OwnerId = "owner-1";
    private const string UploadedUrl = "https://cdn.test/uploaded.jpg";

    private readonly ICarRepository _carRepo = Substitute.For<ICarRepository>();
    private readonly IImageRepository _imageRepo = Substitute.For<IImageRepository>();
    private readonly IImageService _imageService = Substitute.For<IImageService>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly IFormFile _file = Substitute.For<IFormFile>();
    private readonly UploadCarImageHandler _handler;

    public UploadCarImageHandlerTests()
    {
        _actor.UserId.Returns(OwnerId);
        _carRepo.GetUserCarByIdAsync(OwnerId, CarId, Arg.Any<CancellationToken>())
            .Returns(CarImageTestHelpers.BuildCar(CarId, OwnerId));
        _imageService.GetImageLimitsAsync(OwnerId, Arg.Any<CancellationToken>())
            .Returns(new ImageLimitsDto
            {
                MaxImagesPerPost = 10,
                MaxImagesPerCar = 20,
                MaxFileSizeBytes = 5_000_000,
                AllowedFormats = ["image/jpeg"],
                SubscriptionTier = SubscriptionTier.Gold
            });
        _imageService.UploadImageAsync(
                _file, ImageContext.Car, Arg.Any<CancellationToken>(), CarId, Arg.Any<string?>())
            .Returns(new ImageUploadDto
            {
                ImageUrl = UploadedUrl,
                FileName = "uploaded.jpg",
                FileSizeBytes = 1234,
                Width = 800,
                Height = 600,
                ContentType = "image/jpeg",
                UploadedAt = DateTime.UtcNow
            });
        _imageRepo.When(r => r.AddCarImageAsync(Arg.Any<UserCarImage>(), Arg.Any<CancellationToken>()))
            .Do(ci => ci.ArgAt<UserCarImage>(0).Id = 42);

        _handler = new UploadCarImageHandler(_carRepo, _imageRepo, _imageService, _actor);
    }

    [Fact]
    public async Task Handle_CarNotFound_ReturnsNotFound()
    {
        _carRepo.GetUserCarByIdAsync(OwnerId, CarId, Arg.Any<CancellationToken>()).Returns((UserCar?)null);

        var result = await _handler.Handle(new UploadCarImageCommand(CarId, _file, null), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        await _imageService.DidNotReceive().UploadImageAsync(
            Arg.Any<IFormFile>(), Arg.Any<ImageContext>(), Arg.Any<CancellationToken>(), Arg.Any<int?>(), Arg.Any<string?>());
    }

    [Fact]
    public async Task Handle_LimitReached_ReturnsValidation()
    {
        _imageRepo.GetImageCountByContextAsync(ImageContext.Car.ToString(), CarId, Arg.Any<CancellationToken>())
            .Returns(20);

        var result = await _handler.Handle(new UploadCarImageCommand(CarId, _file, null), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.Validation);
    }

    [Fact]
    public async Task Handle_FirstImage_IsMarkedPrimaryAndUpdatesCarUrl()
    {
        _imageRepo.GetImageCountByContextAsync(ImageContext.Car.ToString(), CarId, Arg.Any<CancellationToken>())
            .Returns(0);

        UserCarImage? captured = null;
        _imageRepo.When(r => r.AddCarImageAsync(Arg.Any<UserCarImage>(), Arg.Any<CancellationToken>()))
            .Do(ci =>
            {
                var img = ci.ArgAt<UserCarImage>(0);
                img.Id = 42;
                captured = img;
            });

        var result = await _handler.Handle(new UploadCarImageCommand(CarId, _file, "cap"), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured.Should().NotBeNull();
        captured!.IsPrimary.Should().BeTrue();
        captured.SortOrder.Should().Be(0);
        captured.ImageUrl.Should().Be(UploadedUrl);
        await _imageRepo.Received(1).SetPrimaryImageAsync(CarId, 42, Arg.Any<CancellationToken>());
        await _carRepo.Received(1).UpdatePrimaryImageUrlAsync(CarId, UploadedUrl, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_SubsequentImage_IsNotPrimary_AndCarUrlIsUntouched()
    {
        _imageRepo.GetImageCountByContextAsync(ImageContext.Car.ToString(), CarId, Arg.Any<CancellationToken>())
            .Returns(3);

        UserCarImage? captured = null;
        _imageRepo.When(r => r.AddCarImageAsync(Arg.Any<UserCarImage>(), Arg.Any<CancellationToken>()))
            .Do(ci =>
            {
                var img = ci.ArgAt<UserCarImage>(0);
                img.Id = 42;
                captured = img;
            });

        var result = await _handler.Handle(new UploadCarImageCommand(CarId, _file, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        captured!.IsPrimary.Should().BeFalse();
        captured.SortOrder.Should().Be(3);
        await _imageRepo.DidNotReceive().SetPrimaryImageAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        await _carRepo.DidNotReceive().UpdatePrimaryImageUrlAsync(Arg.Any<int>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }
}
