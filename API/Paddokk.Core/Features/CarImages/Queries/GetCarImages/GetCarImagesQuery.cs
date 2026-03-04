using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Image;

namespace Paddokk.Core.Features.CarImages.Queries.GetCarImages;

public record GetCarImagesQuery(int CarId) : IQuery<Result<CarImagesResponse>>;
