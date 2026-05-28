using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.CarImages;

internal static class CarImageTestHelpers
{
    internal static UserCar BuildCar(int id = 1, string ownerId = "owner-1") => new()
    {
        Id = id,
        PrincipalId = ownerId,
        Slug = $"car-{id}",
        IsPublic = true
    };

    internal static UserCarImage BuildImage(
        int id,
        int carId = 1,
        bool isPrimary = false,
        int sortOrder = 0,
        string imageUrl = "https://cdn.test/img.jpg") => new()
    {
        Id = id,
        UserCarId = carId,
        ImageUrl = imageUrl,
        IsPrimary = isPrimary,
        SortOrder = sortOrder,
        CreatedAt = DateTime.UtcNow,
        ContentType = "image/jpeg"
    };
}
