using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Journeys.Commands.DeleteJourneyPostImage;

public record DeleteJourneyPostImageCommand(int JourneyId, string ImageUrl) : ICommand<Result>;
