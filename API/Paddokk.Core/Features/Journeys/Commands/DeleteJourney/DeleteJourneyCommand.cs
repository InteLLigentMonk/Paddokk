using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Journeys.Commands.DeleteJourney;

public record DeleteJourneyCommand(int JourneyId) : ICommand<Result>;
