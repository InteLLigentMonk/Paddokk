using Paddokk.Core.Models.DTOs.Car;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Cars;

internal static class CarMapping
{
    internal static CarMakeDto ToMakeDto(CarMake make) => new()
    {
        Id = make.Id,
        Name = make.Name,
        Country = make.Country,
        Group = make.Group,
        ModelCount = make.Models.Count
    };

    internal static CarModelDto ToModelDto(CarModel model) => new()
    {
        Id = model.Id,
        Name = model.Name,
        CarMakeId = model.CarMakeId,
        CarMakeName = model.CarMake.Name,
        GenerationCount = model.Generations.Count
    };

    internal static CarGenerationDto ToGenerationDto(CarGeneration generation) => new()
    {
        Id = generation.Id,
        Name = generation.Name,
        StartYear = generation.StartYear,
        EndYear = generation.EndYear,
        CarModelId = generation.CarModelId,
        CarModelName = generation.CarModel.Name
    };

    internal static UserCarDto ToUserCarDto(UserCar car, string? currentUserId = null) => new()
    {
        Id = car.Id,
        PrincipalId = car.PrincipalId,
        OwnerUsername = car.User?.Username ?? string.Empty,
        OwnerAvatarUrl = car.User?.AvatarUrl,
        Slug = car.Slug,
        IsPublic = car.IsPublic,
        CarMakeId = car.CarMakeId,
        CarMakeName = car.CarMake?.Name,
        CarModelId = car.CarModelId,
        CarModelName = car.CarModel?.Name,
        CarGenerationId = car.CarGenerationId,
        CarGenerationName = car.CarGeneration?.Name,
        Year = car.Year,
        IsCustomBuild = car.IsCustomBuild,
        CustomBuildName = car.CustomBuildName,
        Nickname = car.Nickname,
        Color = car.Color,
        Region = car.Region,
        Drive = car.Drive,
        Engine = car.Engine,
        OdometerKm = car.OdometerKm,
        OwnerNote = car.OwnerNote,
        SpecsByCategory = car.SpecsByCategory.Select(s => new CarSpecCategoryDto
        {
            Category = s.Category,
            Items = s.Items,
        }).ToList(),
        PrimaryImageUrl = car.PrimaryImageUrl,
        IsPrimary = car.IsPrimary,
        CreatedAt = car.CreatedAt,
        UpdatedAt = car.UpdatedAt,
        JourneyCount = car.Journeys.Count,
        LikeCount = car.Likes.Count,
        SubscriberCount = car.Subscriptions.Count(s => s.IsActive),
        IsLiked = currentUserId is not null && car.Likes.Any(l => l.UserId == currentUserId),
        IsSubscribed = currentUserId is not null && car.Subscriptions.Any(s => s.UserId == currentUserId && s.IsActive),
        IsOwner = currentUserId is not null && car.PrincipalId == currentUserId
    };
}
