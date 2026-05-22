using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.CarImages.Commands.DeleteCarImage;

public sealed class DeleteCarImageHandler(
    ICarRepository carRepository,
    IImageRepository imageRepository,
    IImageService imageService,
    IActorResolver actor)
    : IRequestHandler<DeleteCarImageCommand, Result>
{
    public async Task<Result> Handle(DeleteCarImageCommand command, CancellationToken ct)
    {
        var car = await carRepository.GetUserCarByIdAsync(actor.UserId, command.CarId, ct);
        if (car is null)
            return Result.Failure(Error.NotFound($"Car {command.CarId} not found"));

        var image = await imageRepository.GetCarImageByIdAsync(command.ImageId, actor.UserId, ct);
        if (image is null || image.UserCarId != command.CarId)
            return Result.Failure(Error.NotFound($"Image {command.ImageId} not found for car {command.CarId}"));

        await imageService.DeleteImageAsync(image.ImageUrl, ct);

        if (image.IsPrimary)
        {
            var nextPrimary = await imageRepository.GetNextPrimaryImageAsync(image.UserCarId, image.Id, ct);
            if (nextPrimary is not null)
                await imageRepository.SetPrimaryImageAsync(image.UserCarId, nextPrimary.Id, ct);

            await carRepository.UpdatePrimaryImageUrlAsync(image.UserCarId, nextPrimary?.ImageUrl, ct);
        }

        await imageRepository.DeleteCarImageAsync(image.Id, ct);

        return Result.Success();
    }
}
