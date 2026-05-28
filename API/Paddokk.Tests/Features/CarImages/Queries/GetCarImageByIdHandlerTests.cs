using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.CarImages.Queries.GetCarImageById;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.CarImages.Queries;

public class GetCarImageByIdHandlerTests
{
    private const int CarId = 1;
    private const int ImageId = 99;

    private readonly IImageRepository _imageRepo = Substitute.For<IImageRepository>();
    private readonly GetCarImageByIdHandler _handler;

    public GetCarImageByIdHandlerTests()
    {
        _handler = new GetCarImageByIdHandler(_imageRepo);
    }

    [Fact]
    public async Task Handle_NotFound_ReturnsNotFound()
    {
        _imageRepo.GetCarImageByIdAsync(ImageId, Arg.Any<CancellationToken>()).Returns((UserCarImage?)null);

        var result = await _handler.Handle(new GetCarImageByIdQuery(CarId, ImageId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_ImageBelongsToDifferentCar_ReturnsNotFound()
    {
        _imageRepo.GetCarImageByIdAsync(ImageId, Arg.Any<CancellationToken>())
            .Returns(CarImageTestHelpers.BuildImage(ImageId, carId: 999));

        var result = await _handler.Handle(new GetCarImageByIdQuery(CarId, ImageId), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_MatchingImage_ReturnsDto()
    {
        var image = CarImageTestHelpers.BuildImage(ImageId, carId: CarId, isPrimary: true);
        _imageRepo.GetCarImageByIdAsync(ImageId, Arg.Any<CancellationToken>()).Returns(image);

        var result = await _handler.Handle(new GetCarImageByIdQuery(CarId, ImageId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Id.Should().Be(ImageId);
        result.Value.UserCarId.Should().Be(CarId);
        result.Value.IsPrimary.Should().BeTrue();
    }
}
