using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.CarImages.Commands.SetPrimaryImage;

public sealed class SetPrimaryImageHandler(
    ICarRepository carRepository,
    IImageRepository imageRepository,
    IActorResolver actor)
    : IRequestHandler<SetPrimaryImageCommand, Result>
{
    public async Task<Result> Handle(SetPrimaryImageCommand command, CancellationToken ct)
    {
        var car = await carRepository.GetUserCarByIdAsync(actor.UserId, command.CarId, ct);
        if (car is null)
            return Result.Failure(Error.NotFound($"Car {command.CarId} not found"));

        var image = await imageRepository.GetCarImageByIdAsync(command.ImageId, ct);
        if (image is null || image.UserCarId != command.CarId)
            return Result.Failure(Error.NotFound($"Image {command.ImageId} not found for car {command.CarId}"));

        await imageRepository.SetPrimaryImageAsync(command.CarId, command.ImageId, ct);
        await carRepository.UpdatePrimaryImageUrlAsync(command.CarId, image.ImageUrl, ct);

        return Result.Success();
    }
}
