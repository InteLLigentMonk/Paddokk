using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Image;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.CarImages.Commands.UploadCarImage;

public sealed class UploadCarImageHandler(
    ICarRepository carRepository,
    IImageRepository imageRepository,
    IImageService imageService,
    IActorResolver actor)
    : IRequestHandler<UploadCarImageCommand, Result<CarImageDto>>
{
    public async Task<Result<CarImageDto>> Handle(UploadCarImageCommand command, CancellationToken ct)
    {
        var car = await carRepository.GetUserCarByIdAsync(actor.UserId, command.CarId, ct);
        if (car is null)
            return Result<CarImageDto>.Failure(Error.NotFound($"Car {command.CarId} not found"));

        var limits = await imageService.GetImageLimitsAsync(actor.UserId, ct);
        var imageCount = await imageRepository.GetImageCountByContextAsync(ImageContext.Car.ToString(), command.CarId, ct);

        if (imageCount >= limits.MaxImagesPerCar)
            return Result<CarImageDto>.Failure(Error.Validation($"Image limit reached ({limits.MaxImagesPerCar}) for this car"));

        var uploaded = await imageService.UploadImageAsync(command.File, ImageContext.Car, ct, command.CarId, command.Caption);

        var isPrimary = imageCount == 0;

        var carImage = new UserCarImage
        {
            UserCarId = command.CarId,
            ImageUrl = uploaded.ImageUrl,
            Caption = command.Caption,
            SortOrder = imageCount,
            IsPrimary = isPrimary,
            FileSizeBytes = uploaded.FileSizeBytes,
            Width = uploaded.Width,
            Height = uploaded.Height,
            ContentType = uploaded.ContentType,
            CreatedAt = DateTime.UtcNow
        };

        await imageRepository.AddCarImageAsync(carImage, ct);

        if (isPrimary)
        {
            await imageRepository.SetPrimaryImageAsync(command.CarId, carImage.Id, ct);
            await carRepository.UpdatePrimaryImageUrlAsync(command.CarId, uploaded.ImageUrl, ct);
        }

        return Result<CarImageDto>.Success(CarImageMapping.ToDto(carImage));
    }
}
