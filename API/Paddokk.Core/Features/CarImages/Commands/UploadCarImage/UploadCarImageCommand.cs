using Microsoft.AspNetCore.Http;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Image;

namespace Paddokk.Core.Features.CarImages.Commands.UploadCarImage;

public record UploadCarImageCommand(int CarId, IFormFile File, string? Caption) : ICommand<Result<CarImageDto>>;
