using Paddokk.Core.Models.DTOs.Image;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.CarImages;

internal static class CarImageMapping
{
    internal static CarImageDto ToDto(UserCarImage image) => new()
    {
        Id = image.Id,
        UserCarId = image.UserCarId,
        ImageUrl = image.ImageUrl,
        Caption = image.Caption,
        SortOrder = image.SortOrder,
        IsPrimary = image.IsPrimary,
        CreatedAt = image.CreatedAt
    };
}
