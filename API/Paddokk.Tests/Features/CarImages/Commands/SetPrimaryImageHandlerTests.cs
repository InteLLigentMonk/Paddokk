using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.CarImages.Commands.SetPrimaryImage;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.CarImages.Commands;

public class SetPrimaryImageHandlerTests
{
    private const int CarId = 1;
    private const int ImageId = 99;
    private const string OwnerId = "owner-1";

    private readonly ICarRepository _carRepo = Substitute.For<ICarRepository>();
    private readonly IImageRepository _imageRepo = Substitute.For<IImageRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly SetPrimaryImageHandler _handler;

    public SetPrimaryImageHandlerTests()
    {
        _actor.UserId.Returns(OwnerId);
        _handler = new SetPrimaryImageHandler(_carRepo, _imageRepo, _actor);
    }

    [Fact]
    public async Task Handle_CarNotFound_ReturnsNotFound()
    {
        _carRepo.GetUserCarByIdAsync(OwnerId, CarId, Arg.Any<CancellationToken>()).Returns((UserCar?)null);

        var result = await _handler.Handle(new SetPrimaryImageCommand(CarId, ImageId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        await _imageRepo.DidNotReceive().SetPrimaryImageAsync(
            Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ImageNotFound_ReturnsNotFound()
    {
        _carRepo.GetUserCarByIdAsync(OwnerId, CarId, Arg.Any<CancellationToken>())
            .Returns(CarImageTestHelpers.BuildCar(CarId, OwnerId));
        _imageRepo.GetCarImageByIdAsync(ImageId, Arg.Any<CancellationToken>())
            .Returns((UserCarImage?)null);

        var result = await _handler.Handle(new SetPrimaryImageCommand(CarId, ImageId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ImageBelongsToDifferentCar_ReturnsNotFound()
    {
        _carRepo.GetUserCarByIdAsync(OwnerId, CarId, Arg.Any<CancellationToken>())
            .Returns(CarImageTestHelpers.BuildCar(CarId, OwnerId));
        var foreignImage = CarImageTestHelpers.BuildImage(ImageId, carId: 999);
        _imageRepo.GetCarImageByIdAsync(ImageId, Arg.Any<CancellationToken>()).Returns(foreignImage);

        var result = await _handler.Handle(new SetPrimaryImageCommand(CarId, ImageId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
        await _imageRepo.DidNotReceive().SetPrimaryImageAsync(
            Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_Valid_DelegatesPrimaryInvariantToRepoAndSyncsCarUrl()
    {
        const string url = "https://cdn.test/new-primary.jpg";
        _carRepo.GetUserCarByIdAsync(OwnerId, CarId, Arg.Any<CancellationToken>())
            .Returns(CarImageTestHelpers.BuildCar(CarId, OwnerId));
        _imageRepo.GetCarImageByIdAsync(ImageId, Arg.Any<CancellationToken>())
            .Returns(CarImageTestHelpers.BuildImage(ImageId, carId: CarId, imageUrl: url));

        var result = await _handler.Handle(new SetPrimaryImageCommand(CarId, ImageId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _imageRepo.Received(1).SetPrimaryImageAsync(CarId, ImageId, Arg.Any<CancellationToken>());
        await _carRepo.Received(1).UpdatePrimaryImageUrlAsync(CarId, url, Arg.Any<CancellationToken>());
    }
}
