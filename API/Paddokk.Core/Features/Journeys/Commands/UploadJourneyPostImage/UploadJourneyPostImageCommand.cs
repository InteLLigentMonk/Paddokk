using Microsoft.AspNetCore.Http;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Image;

namespace Paddokk.Core.Features.Journeys.Commands.UploadJourneyPostImage;

public record UploadJourneyPostImageCommand(int JourneyId, IFormFile File) : ICommand<Result<ImageUploadDto>>;
