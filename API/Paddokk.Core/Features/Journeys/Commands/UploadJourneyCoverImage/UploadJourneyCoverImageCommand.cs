using Microsoft.AspNetCore.Http;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Commands.UploadJourneyCoverImage;

public record UploadJourneyCoverImageCommand(int JourneyId, IFormFile File) : ICommand<Result<JourneyDto>>;
