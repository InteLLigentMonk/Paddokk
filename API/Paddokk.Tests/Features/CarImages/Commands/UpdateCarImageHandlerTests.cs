using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.CarImages.Commands.UpdateCarImage;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.CarImages.Commands;

public class UpdateCarImageHandlerTests
{
    private const int CarId = 1;
    private const int ImageId = 99;
    private const string OwnerId = "owner-1";

    private readonly IImageRepository _imageRepo = Substitute.For<IImageRepository>();
    private readonly ICarRepository _carRepo = Substitute.For<ICarRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly UpdateCarImageHandler _handler;

    public UpdateCarImageHandlerTests()
    {
        _actor.UserId.Returns(OwnerId);
        _handler = new UpdateCarImageHandler(_imageRepo, _carRepo, _actor);
    }

    [Fact]
    public async Task Handle_ImageNotFound_ReturnsNotFound()
    {
        _imageRepo.GetCarImageByIdAsync(ImageId, OwnerId, Arg.Any<CancellationToken>()).Returns((UserCarImage?)null);

        var result = await _handler.Handle(
            new UpdateCarImageCommand(CarId, ImageId, "cap", null, null), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ImageBelongsToDifferentCar_ReturnsNotFound()
    {
        var foreign = CarImageTestHelpers.BuildImage(ImageId, carId: 999);
        _imageRepo.GetCarImageByIdAsync(ImageId, OwnerId, Arg.Any<CancellationToken>()).Returns(foreign);

        var result = await _handler.Handle(
            new UpdateCarImageCommand(CarId, ImageId, "cap", null, null), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_CaptionAndSortOrderOnly_PersistsWithoutTouchingPrimary()
    {
        var image = CarImageTestHelpers.BuildImage(ImageId, carId: CarId, isPrimary: false, sortOrder: 0);
        _imageRepo.GetCarImageByIdAsync(ImageId, OwnerId, Arg.Any<CancellationToken>()).Returns(image);

        var result = await _handler.Handle(
            new UpdateCarImageCommand(CarId, ImageId, "new caption", 5, null), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        image.Caption.Should().Be("new caption");
        image.SortOrder.Should().Be(5);
        await _imageRepo.Received(1).UpdateCarImageAsync(image, Arg.Any<CancellationToken>());
        await _imageRepo.DidNotReceive().SetPrimaryImageAsync(
            Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        await _carRepo.DidNotReceive().UpdatePrimaryImageUrlAsync(
            Arg.Any<int>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_PromoteToPrimary_DelegatesPrimaryInvariantToRepoAndSyncsCarUrl()
    {
        const string url = "https://cdn.test/promote.jpg";
        var image = CarImageTestHelpers.BuildImage(ImageId, carId: CarId, isPrimary: false, imageUrl: url);
        _imageRepo.GetCarImageByIdAsync(ImageId, OwnerId, Arg.Any<CancellationToken>()).Returns(image);

        var result = await _handler.Handle(
            new UpdateCarImageCommand(CarId, ImageId, null, null, IsPrimary: true), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _imageRepo.Received(1).SetPrimaryImageAsync(CarId, ImageId, Arg.Any<CancellationToken>());
        await _carRepo.Received(1).UpdatePrimaryImageUrlAsync(CarId, url, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_AlreadyPrimary_DoesNotReissuePrimaryUpdate()
    {
        var image = CarImageTestHelpers.BuildImage(ImageId, carId: CarId, isPrimary: true);
        _imageRepo.GetCarImageByIdAsync(ImageId, OwnerId, Arg.Any<CancellationToken>()).Returns(image);

        var result = await _handler.Handle(
            new UpdateCarImageCommand(CarId, ImageId, null, null, IsPrimary: true), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _imageRepo.DidNotReceive().SetPrimaryImageAsync(
            Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        await _carRepo.DidNotReceive().UpdatePrimaryImageUrlAsync(
            Arg.Any<int>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }
}
