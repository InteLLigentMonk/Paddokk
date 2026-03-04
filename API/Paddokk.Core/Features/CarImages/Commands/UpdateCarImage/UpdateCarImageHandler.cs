using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Image;

namespace Paddokk.Core.Features.CarImages.Commands.UpdateCarImage;

public sealed class UpdateCarImageHandler(
    IImageRepository imageRepository,
    ICarRepository carRepository,
    IActorResolver actor)
    : IRequestHandler<UpdateCarImageCommand, Result<CarImageDto>>
{
    public async Task<Result<CarImageDto>> Handle(UpdateCarImageCommand command, CancellationToken ct)
    {
        var image = await imageRepository.GetCarImageByIdAsync(command.ImageId, actor.UserId, ct);

        if (image is null || image.UserCarId != command.CarId)
            return Result<CarImageDto>.Failure(Error.NotFound($"Image {command.ImageId} not found for car {command.CarId}"));

        if (command.Caption is not null)
            image.Caption = command.Caption;

        if (command.SortOrder.HasValue)
            image.SortOrder = command.SortOrder.Value;

        if (command.IsPrimary == true && !image.IsPrimary)
        {
            await imageRepository.SetPrimaryImageAsync(image.UserCarId, image.Id, ct);
            await carRepository.UpdatePrimaryImageUrlAsync(image.UserCarId, image.MediumUrl, ct);
        }

        await imageRepository.UpdateCarImageAsync(image, ct);

        return Result<CarImageDto>.Success(CarImageMapping.ToDto(image));
    }
}
