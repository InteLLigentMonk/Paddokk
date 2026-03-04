using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Image;

namespace Paddokk.Core.Features.CarImages.Commands.UpdateCarImage;

public record UpdateCarImageCommand(
    int CarId,
    int ImageId,
    string? Caption,
    int? SortOrder,
    bool? IsPrimary
) : ICommand<Result<CarImageDto>>;
