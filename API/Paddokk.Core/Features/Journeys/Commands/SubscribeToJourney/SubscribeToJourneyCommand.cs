using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Journeys.Commands.SubscribeToJourney;

public record SubscribeToJourneyCommand(int JourneyId) : ICommand<Result>;
