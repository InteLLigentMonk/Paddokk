using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Image;

namespace Paddokk.Core.Features.CarImages.Queries.GetCarImageById;

public record GetCarImageByIdQuery(int CarId, int ImageId) : IQuery<Result<CarImageDto>>;
