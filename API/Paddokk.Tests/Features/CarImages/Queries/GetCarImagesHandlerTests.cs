using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.CarImages.Queries.GetCarImages;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.CarImages.Queries;

public class GetCarImagesHandlerTests
{
    private const int CarId = 1;

    private readonly IImageRepository _imageRepo = Substitute.For<IImageRepository>();
    private readonly GetCarImagesHandler _handler;

    public GetCarImagesHandlerTests()
    {
        _handler = new GetCarImagesHandler(_imageRepo);
    }

    [Fact]
    public async Task Handle_NoImages_ReturnsEmptyList()
    {
        _imageRepo.GetCarImagesAsync(CarId, Arg.Any<CancellationToken>())
            .Returns(Array.Empty<UserCarImage>());

        var result = await _handler.Handle(new GetCarImagesQuery(CarId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Images.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_MultipleImages_PreservesRepoOrderAndMapsFlags()
    {
        var images = new[]
        {
            CarImageTestHelpers.BuildImage(1, CarId, isPrimary: true, sortOrder: 0),
            CarImageTestHelpers.BuildImage(2, CarId, isPrimary: false, sortOrder: 1),
            CarImageTestHelpers.BuildImage(3, CarId, isPrimary: false, sortOrder: 2)
        };
        _imageRepo.GetCarImagesAsync(CarId, Arg.Any<CancellationToken>()).Returns(images);

        var result = await _handler.Handle(new GetCarImagesQuery(CarId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value!.Images.Should().HaveCount(3);
        result.Value.Images.Select(i => i.Id).Should().Equal(1, 2, 3);
        result.Value.Images.Count(i => i.IsPrimary).Should().Be(1);
    }
}
