using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Journeys.Commands.LikeJourney;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Journeys.Commands;

public class LikeJourneyHandlerTests
{
    private readonly IJourneyRepository _journeyRepo = Substitute.For<IJourneyRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly LikeJourneyHandler _handler;

    public LikeJourneyHandlerTests()
    {
        _handler = new LikeJourneyHandler(_journeyRepo, _actor);
    }

    [Fact]
    public async Task Handle_OwnerLikesOwnJourney_ReturnsConflict()
    {
        _actor.UserId.Returns("owner-1");
        _journeyRepo.GetJourneyByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(JourneyTestHelpers.BuildJourney(id: 1, userId: "owner-1"));

        var result = await _handler.Handle(new LikeJourneyCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.Conflict);
    }

    [Fact]
    public async Task Handle_JourneyNotFound_ReturnsNotFound()
    {
        _actor.UserId.Returns("visitor-1");
        _journeyRepo.GetJourneyByIdAsync(999, Arg.Any<CancellationToken>()).Returns((Journey?)null);

        var result = await _handler.Handle(new LikeJourneyCommand(999), CancellationToken.None);

        result.IsSuccess.Should().BeFalse();
        result.Error!.Type.Should().Be(ErrorType.NotFound);
    }

    [Fact]
    public async Task Handle_NonOwnerLikesJourney_CreatesLike()
    {
        _actor.UserId.Returns("visitor-1");
        _journeyRepo.GetJourneyByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(JourneyTestHelpers.BuildJourney(id: 1, userId: "owner-1"));
        _journeyRepo.GetLikeAsync("visitor-1", 1, Arg.Any<CancellationToken>()).Returns((JourneyLike?)null);

        var result = await _handler.Handle(new LikeJourneyCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _journeyRepo.Received(1).CreateLikeAsync(
            Arg.Is<JourneyLike>(l => l.UserId == "visitor-1" && l.JourneyId == 1),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_NonOwnerAlreadyLiked_IsIdempotent()
    {
        _actor.UserId.Returns("visitor-1");
        _journeyRepo.GetJourneyByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(JourneyTestHelpers.BuildJourney(id: 1, userId: "owner-1"));
        _journeyRepo.GetLikeAsync("visitor-1", 1, Arg.Any<CancellationToken>())
            .Returns(new JourneyLike { UserId = "visitor-1", JourneyId = 1, CreatedAt = DateTime.UtcNow });

        var result = await _handler.Handle(new LikeJourneyCommand(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        await _journeyRepo.DidNotReceive().CreateLikeAsync(Arg.Any<JourneyLike>(), Arg.Any<CancellationToken>());
    }
}
