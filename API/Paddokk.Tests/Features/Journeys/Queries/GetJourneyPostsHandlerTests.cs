using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Journeys.Queries.GetJourneyPosts;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Journeys.Queries;

public class GetJourneyPostsHandlerTests
{
    private readonly IJourneyRepository _repo = Substitute.For<IJourneyRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly GetJourneyPostsHandler _handler;

    public GetJourneyPostsHandlerTests()
    {
        _handler = new GetJourneyPostsHandler(_repo, _actor);
    }

    private static JourneyPost BuildPost(int id, int journeyId, string authorId) => new()
    {
        Id = id,
        JourneyId = journeyId,
        AuthorId = authorId,
        TextContent = "post body",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        Author = JourneyTestHelpers.BuildUser(authorId),
        Images = [],
        Comments = []
    };

    [Fact]
    public async Task Handle_PrivateJourney_AnonymousActor_ReturnsEmpty()
    {
        var journey = JourneyTestHelpers.BuildJourney(id: 1, userId: "owner-1");
        journey.IsPublic = false;
        _repo.GetJourneyByIdAsync(1, Arg.Any<CancellationToken>()).Returns(journey);
        _repo.GetJourneyPostsAsync(1, 0, 20, Arg.Any<CancellationToken>())
            .Returns([BuildPost(10, 1, "owner-1")]);
        _actor.IsAuthenticated.Returns(false);

        var result = await _handler.Handle(new GetJourneyPostsQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_PrivateJourney_NonOwnerActor_ReturnsEmpty()
    {
        var journey = JourneyTestHelpers.BuildJourney(id: 1, userId: "owner-1");
        journey.IsPublic = false;
        _repo.GetJourneyByIdAsync(1, Arg.Any<CancellationToken>()).Returns(journey);
        _repo.GetJourneyPostsAsync(1, 0, 20, Arg.Any<CancellationToken>())
            .Returns([BuildPost(10, 1, "owner-1")]);
        _actor.IsAuthenticated.Returns(true);
        _actor.UserId.Returns("visitor-1");

        var result = await _handler.Handle(new GetJourneyPostsQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEmpty();
    }

    [Fact]
    public async Task Handle_PrivateJourney_OwnerActor_ReturnsFullList()
    {
        var journey = JourneyTestHelpers.BuildJourney(id: 1, userId: "owner-1");
        journey.IsPublic = false;
        _repo.GetJourneyByIdAsync(1, Arg.Any<CancellationToken>()).Returns(journey);
        _repo.GetJourneyPostsAsync(1, 0, 20, Arg.Any<CancellationToken>())
            .Returns([BuildPost(10, 1, "owner-1"), BuildPost(11, 1, "owner-1")]);
        _actor.IsAuthenticated.Returns(true);
        _actor.UserId.Returns("owner-1");

        var result = await _handler.Handle(new GetJourneyPostsQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(2);
    }

    [Fact]
    public async Task Handle_PublicJourney_AnonymousActor_ReturnsFullList()
    {
        var journey = JourneyTestHelpers.BuildJourney(id: 1, userId: "owner-1");
        journey.IsPublic = true;
        _repo.GetJourneyByIdAsync(1, Arg.Any<CancellationToken>()).Returns(journey);
        _repo.GetJourneyPostsAsync(1, 0, 20, Arg.Any<CancellationToken>())
            .Returns([BuildPost(10, 1, "owner-1")]);
        _actor.IsAuthenticated.Returns(false);

        var result = await _handler.Handle(new GetJourneyPostsQuery(1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
    }
}