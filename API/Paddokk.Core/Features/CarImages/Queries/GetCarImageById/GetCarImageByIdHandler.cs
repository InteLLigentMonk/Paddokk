using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Image;

namespace Paddokk.Core.Features.CarImages.Queries.GetCarImageById;

public sealed class GetCarImageByIdHandler(IImageRepository imageRepository)
    : IRequestHandler<GetCarImageByIdQuery, Result<CarImageDto>>
{
    public async Task<Result<CarImageDto>> Handle(GetCarImageByIdQuery query, CancellationToken ct)
    {
        var image = await imageRepository.GetCarImageByIdAsync(query.ImageId, ct);

        if (image is null || image.UserCarId != query.CarId)
            return Result<CarImageDto>.Failure(Error.NotFound($"Image {query.ImageId} not found for car {query.CarId}"));

        return Result<CarImageDto>.Success(CarImageMapping.ToDto(image));
    }
}
