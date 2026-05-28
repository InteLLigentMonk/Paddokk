using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.CarImages.Commands.DeleteCarImage;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.CarImages.Commands;

public class DeleteCarImageHandlerTests
{
    private const int CarId = 1;
    private const int ImageId = 99;
    private const string OwnerId = "owner-1";

    private readonly ICarRepository _carRepo = Substitute.For<ICarRepository>();
    private readonly IImageRepository _imageRepo = Substitute.For<IImageRepository>();
    private readonly IImageService _imageService = Substitute.For<IImageService>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly DeleteCarImageHandler _handler;

    public DeleteCarImageHandlerTests()
    {
        _actor.UserId.Returns(OwnerId);
        _handler = new DeleteCarImageHandler(_carRepo, _imageRepo, _imageService, _actor);
    }

    [Fact]
    public async Task Handle_CarNotFound_ReturnsNotFound()
    {
        _carRepo.GetUserCarByIdAsync(OwnerId, CarId, Arg.Any<CancellationToken>()).Returns((UserCar?)null);

        var result = await _handler.Handle(new DeleteCarImageCommand(CarId, ImageId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        await _imageService.DidNotReceive().DeleteImageAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ImageNotFound_ReturnsNotFound()
    {
        _carRepo.GetUserCarByIdAsync(OwnerId, CarId, Arg.Any<CancellationToken>())
            .Returns(CarImageTestHelpers.BuildCar(CarId, OwnerId));
        _imageRepo.GetCarImageByIdAsync(ImageId, OwnerId, Arg.Any<CancellationToken>())
            .Returns((UserCarImage?)null);

        var result = await _handler.Handle(new DeleteCarImageCommand(CarId, ImageId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_DeleteNonPrimary_DoesNotPromoteOrChangeCarUrl()
    {
        _carRepo.GetUserCarByIdAsync(OwnerId, CarId, Arg.Any<CancellationToken>())
            .Returns(CarImageTestHelpers.BuildCar(CarId, OwnerId));
        var image = CarImageTestHelpers.BuildImage(ImageId, carId: CarId, isPrimary: false);
        _imageRepo.GetCarImageByIdAsync(ImageId, OwnerId, Arg.Any<CancellationToken>()).Returns(image);

        var result = await _handler.Handle(new DeleteCarImageCommand(CarId, ImageId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _imageService.Received(1).DeleteImageAsync(image.ImageUrl, Arg.Any<CancellationToken>());
        await _imageRepo.Received(1).DeleteCarImageAsync(ImageId, Arg.Any<CancellationToken>());
        await _imageRepo.DidNotReceive().GetNextPrimaryImageAsync(
            Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        await _carRepo.DidNotReceive().UpdatePrimaryImageUrlAsync(
            Arg.Any<int>(), Arg.Any<string?>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DeletePrimary_PromotesNextAndSyncsCarUrl()
    {
        _carRepo.GetUserCarByIdAsync(OwnerId, CarId, Arg.Any<CancellationToken>())
            .Returns(CarImageTestHelpers.BuildCar(CarId, OwnerId));
        var primary = CarImageTestHelpers.BuildImage(ImageId, carId: CarId, isPrimary: true);
        var next = CarImageTestHelpers.BuildImage(id: 200, carId: CarId, imageUrl: "https://cdn.test/next.jpg");
        _imageRepo.GetCarImageByIdAsync(ImageId, OwnerId, Arg.Any<CancellationToken>()).Returns(primary);
        _imageRepo.GetNextPrimaryImageAsync(CarId, ImageId, Arg.Any<CancellationToken>()).Returns(next);

        var result = await _handler.Handle(new DeleteCarImageCommand(CarId, ImageId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _imageRepo.Received(1).SetPrimaryImageAsync(CarId, next.Id, Arg.Any<CancellationToken>());
        await _carRepo.Received(1).UpdatePrimaryImageUrlAsync(CarId, next.ImageUrl, Arg.Any<CancellationToken>());
        await _imageRepo.Received(1).DeleteCarImageAsync(ImageId, Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_DeleteLastPrimary_ClearsCarUrl()
    {
        _carRepo.GetUserCarByIdAsync(OwnerId, CarId, Arg.Any<CancellationToken>())
            .Returns(CarImageTestHelpers.BuildCar(CarId, OwnerId));
        var primary = CarImageTestHelpers.BuildImage(ImageId, carId: CarId, isPrimary: true);
        _imageRepo.GetCarImageByIdAsync(ImageId, OwnerId, Arg.Any<CancellationToken>()).Returns(primary);
        _imageRepo.GetNextPrimaryImageAsync(CarId, ImageId, Arg.Any<CancellationToken>())
            .Returns((UserCarImage?)null);

        var result = await _handler.Handle(new DeleteCarImageCommand(CarId, ImageId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _imageRepo.DidNotReceive().SetPrimaryImageAsync(
            Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
        await _carRepo.Received(1).UpdatePrimaryImageUrlAsync(CarId, null, Arg.Any<CancellationToken>());
        await _imageRepo.Received(1).DeleteCarImageAsync(ImageId, Arg.Any<CancellationToken>());
    }
}
