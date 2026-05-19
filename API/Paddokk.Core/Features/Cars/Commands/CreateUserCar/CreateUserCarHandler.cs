using MediatR;
using Paddokk.Core.Common;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Car;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Cars.Commands.CreateUserCar;

public sealed class CreateUserCarHandler(
    ICarRepository carRepository,
    IUserRepository userRepository,
    IActorResolver actor,
    IHtmlSanitizationService htmlSanitizer,
    SlugGenerator slugGenerator)
    : IRequestHandler<CreateUserCarCommand, Result<UserCarDto>>
{
    public async Task<Result<UserCarDto>> Handle(CreateUserCarCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(actor.UserId, cancellationToken)
            ?? throw new UnauthorizedAccessException("User not found");

        var maxCars = user.SubscriptionTier switch
        {
            SubscriptionTier.Free => 1,
            SubscriptionTier.Silver => 3,
            SubscriptionTier.Gold => 10,
            SubscriptionTier.Platinum => 20,
            SubscriptionTier.Diamond => int.MaxValue,
            _ => 2
        };

        var currentCount = await carRepository.GetUserCarCountAsync(actor.UserId, cancellationToken);
        if (currentCount >= maxCars)
            return Result<UserCarDto>.Failure(Error.Conflict("Car limit reached for current subscription tier"));

        string? makeName = null;
        string? modelName = null;
        string? generationName = null;

        if (!request.IsCustomBuild)
        {
            var make = await carRepository.GetCarMakeByIdAsync(request.CarMakeId!.Value, cancellationToken);
            if (make is null)
                return Result<UserCarDto>.Failure(Error.NotFound($"Car make {request.CarMakeId} not found"));
            makeName = make.Name;

            var model = await carRepository.GetCarModelByIdAsync(request.CarModelId!.Value, cancellationToken);
            if (model is null || model.CarMakeId != request.CarMakeId)
                return Result<UserCarDto>.Failure(Error.NotFound("Car model not found for the specified make"));
            modelName = model.Name;

            if (request.CarGenerationId.HasValue)
            {
                var generation = await carRepository.GetCarGenerationByIdAsync(request.CarGenerationId.Value, cancellationToken);
                if (generation is null || generation.CarModelId != request.CarModelId)
                    return Result<UserCarDto>.Failure(Error.NotFound("Car generation not found for the specified model"));

                if (request.Year < generation.StartYear ||
                    generation.EndYear.HasValue && request.Year > generation.EndYear)
                    return Result<UserCarDto>.Failure(Error.Validation("Car year does not fall within the specified generation range"));

                generationName = generation.Name;
            }
        }

        var searchText = CarSearchTextBuilder.Build(makeName, modelName, generationName, request.CustomBuildName, request.Nickname, request.Year);
        var isPrimary = request.IsPrimary || currentCount == 0;

        var slugSource = BuildSlugSource(request, makeName, modelName);
        var slugCandidate = slugGenerator.Generate(slugSource);
        var slug = await slugGenerator.EnsureUniqueAsync(
            slugCandidate, actor.UserId, carRepository.SlugExistsAsync, cancellationToken);

        var userCar = new UserCar
        {
            PrincipalId = actor.UserId,
            Slug = slug,
            IsPublic = true,
            IsCustomBuild = request.IsCustomBuild,
            CustomBuildName = request.CustomBuildName,
            CarMakeId = request.CarMakeId,
            CarModelId = request.CarModelId,
            CarGenerationId = request.CarGenerationId,
            Year = request.Year,
            Nickname = request.Nickname,
            Color = request.Color,
            Description = htmlSanitizer.Sanitize(request.Description),
            SearchText = searchText,
            IsPrimary = isPrimary,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        if (isPrimary)
            await carRepository.UnsetPrimaryCar(actor.UserId, cancellationToken);

        await carRepository.CreateUserCarAsync(userCar, cancellationToken);

        var created = await carRepository.GetUserCarByIdAsync(actor.UserId, userCar.Id, cancellationToken);
        return Result<UserCarDto>.Success(CarMapping.ToUserCarDto(created!));
    }


    private static string BuildSlugSource(
        CreateUserCarCommand request,
        string? makeName,
        string? modelName)
    {
        if (!string.IsNullOrWhiteSpace(request.Nickname))
            return request.Nickname;

        if (request.IsCustomBuild && !string.IsNullOrWhiteSpace(request.CustomBuildName))
            return request.CustomBuildName;

        var parts = new[] { makeName, modelName, request.Year?.ToString() }
            .Where(p => !string.IsNullOrWhiteSpace(p));
        var combined = string.Join(" ", parts);
        return string.IsNullOrWhiteSpace(combined) ? "car" : combined;
    }
}
