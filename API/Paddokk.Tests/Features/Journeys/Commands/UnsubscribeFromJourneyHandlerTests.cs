using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Journeys.Commands.UnsubscribeFromJourney;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Journeys.Commands;

public class UnsubscribeFromJourneyHandlerTests
{
    private readonly IJourneyRepository _journeyRepo = Substitute.For<IJourneyRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly UnsubscribeFromJourneyHandler _handler;

    public UnsubscribeFromJourneyHandlerTests()
    {
        _handler = new UnsubscribeFromJourneyHandler(_journeyRepo, _actor);
    }

    [Fact]
    public async Task Handle_NoExistingSubscription_IsIdempotent()
    {
        _actor.UserId.Returns("visitor-1");
        _journeyRepo.GetSubscriptionAsync("visitor-1", 1, Arg.Any<CancellationToken>())
            .Returns((JourneySubscription?)null);

        var result = await _handler.Handle(new UnsubscribeFromJourneyCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _journeyRepo.DidNotReceive().UpdateSubscriptionAsync(Arg.Any<JourneySubscription>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ExistingSubscription_SoftDeletes()
    {
        _actor.UserId.Returns("visitor-1");
        var existing = new JourneySubscription { UserId = "visitor-1", JourneyId = 1, IsActive = true };
        _journeyRepo.GetSubscriptionAsync("visitor-1", 1, Arg.Any<CancellationToken>()).Returns(existing);

        var result = await _handler.Handle(new UnsubscribeFromJourneyCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existing.IsActive.Should().BeFalse();
        await _journeyRepo.Received(1).UpdateSubscriptionAsync(existing, Arg.Any<CancellationToken>());
    }
}
