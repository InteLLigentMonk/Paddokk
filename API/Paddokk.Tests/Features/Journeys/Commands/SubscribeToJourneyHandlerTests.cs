using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Journeys.Commands.SubscribeToJourney;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Journeys.Commands;

public class SubscribeToJourneyHandlerTests
{
    private readonly IJourneyRepository _journeyRepo = Substitute.For<IJourneyRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly SubscribeToJourneyHandler _handler;

    public SubscribeToJourneyHandlerTests()
    {
        _handler = new SubscribeToJourneyHandler(_journeyRepo, _actor);
    }

    [Fact]
    public async Task Handle_JourneyNotFound_ReturnsNotFound()
    {
        _actor.UserId.Returns("visitor-1");
        _journeyRepo.GetJourneyByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Journey?)null);

        var result = await _handler.Handle(new SubscribeToJourneyCommand(999), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_OwnerSubscribesToOwnJourney_ReturnsConflict()
    {
        _actor.UserId.Returns("owner-1");
        _journeyRepo.GetJourneyByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(JourneyTestHelpers.BuildJourney(id: 1, userId: "owner-1"));

        var result = await _handler.Handle(new SubscribeToJourneyCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.Conflict);
        result.Error.Code.Should().Be(ErrorCodes.SubscribeToOwnSubject);
    }

    [Fact]
    public async Task Handle_ExistingActiveSubscription_IsNoOp()
    {
        _actor.UserId.Returns("visitor-1");
        _journeyRepo.GetJourneyByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(JourneyTestHelpers.BuildJourney(id: 1, userId: "owner-1"));
        _journeyRepo.GetSubscriptionAsync("visitor-1", 1, Arg.Any<CancellationToken>())
            .Returns(new JourneySubscription { UserId = "visitor-1", JourneyId = 1, IsActive = true });

        var result = await _handler.Handle(new SubscribeToJourneyCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _journeyRepo.DidNotReceive().CreateSubscriptionAsync(Arg.Any<JourneySubscription>(), Arg.Any<CancellationToken>());
        await _journeyRepo.DidNotReceive().UpdateSubscriptionAsync(Arg.Any<JourneySubscription>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_ExistingInactiveSubscription_Reactivates()
    {
        _actor.UserId.Returns("visitor-1");
        _journeyRepo.GetJourneyByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(JourneyTestHelpers.BuildJourney(id: 1, userId: "owner-1"));
        var existing = new JourneySubscription { UserId = "visitor-1", JourneyId = 1, IsActive = false };
        _journeyRepo.GetSubscriptionAsync("visitor-1", 1, Arg.Any<CancellationToken>()).Returns(existing);

        var result = await _handler.Handle(new SubscribeToJourneyCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        existing.IsActive.Should().BeTrue();
        await _journeyRepo.Received(1).UpdateSubscriptionAsync(existing, Arg.Any<CancellationToken>());
        await _journeyRepo.DidNotReceive().CreateSubscriptionAsync(Arg.Any<JourneySubscription>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NoExistingSubscription_CreatesNew()
    {
        _actor.UserId.Returns("visitor-1");
        _journeyRepo.GetJourneyByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(JourneyTestHelpers.BuildJourney(id: 1, userId: "owner-1"));
        _journeyRepo.GetSubscriptionAsync("visitor-1", 1, Arg.Any<CancellationToken>()).Returns((JourneySubscription?)null);

        var result = await _handler.Handle(new SubscribeToJourneyCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _journeyRepo.Received(1).CreateSubscriptionAsync(
            Arg.Is<JourneySubscription>(s => s.UserId == "visitor-1" && s.JourneyId == 1 && s.IsActive),
            Arg.Any<CancellationToken>());
    }
}
