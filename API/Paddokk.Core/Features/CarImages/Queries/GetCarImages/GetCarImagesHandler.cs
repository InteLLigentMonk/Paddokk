using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Image;

namespace Paddokk.Core.Features.CarImages.Queries.GetCarImages;

public sealed class GetCarImagesHandler(IImageRepository imageRepository)
    : IRequestHandler<GetCarImagesQuery, Result<CarImagesResponse>>
{
    public async Task<Result<CarImagesResponse>> Handle(GetCarImagesQuery query, CancellationToken ct)
    {
        var images = await imageRepository.GetCarImagesAsync(query.CarId, ct);
        return Result<CarImagesResponse>.Success(new CarImagesResponse
        {
            Images = [.. images.Select(CarImageMapping.ToDto)]
        });
    }
}
