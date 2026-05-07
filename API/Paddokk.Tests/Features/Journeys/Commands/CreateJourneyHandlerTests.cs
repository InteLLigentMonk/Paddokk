using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Common;
using Paddokk.Core.Features.Journeys.Commands.CreateJourney;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Journeys.Commands;

public class CreateJourneyHandlerTests
{
    private readonly IJourneyRepository _repo = Substitute.For<IJourneyRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly IHtmlSanitizationService _htmlSanitizer = Substitute.For<IHtmlSanitizationService>();
    private readonly SlugGenerator _slugGenerator = new();
    private readonly CreateJourneyHandler _handler;

    public CreateJourneyHandlerTests()
    {
        _actor.UserId.Returns("user-1");
        _htmlSanitizer.Sanitize(Arg.Any<string?>()).Returns(ci => ci.ArgAt<string?>(0));
        _repo.SlugExistsAsync(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
            .Returns(false);
        _handler = new CreateJourneyHandler(_repo, _actor, _htmlSanitizer, _slugGenerator);

        _repo.GetUserAsync("user-1", Arg.Any<CancellationToken>())
            .Returns(JourneyTestHelpers.BuildUser());
        _repo.GetUserJourneyCountAsync("user-1", Arg.Any<CancellationToken>())
            .Returns(0);
        _repo.CreateJourneyAsync(Arg.Any<Journey>(), Arg.Any<CancellationToken>())
            .Returns(1);
    }

    [Fact]
    public async Task Handle_WithTargetCompletedAt_PersistsTargetCompletedAtToEntity()
    {
        // Arrange
        var target = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc);
        var command = new CreateJourneyCommand("My Journey", null, JourneyCategory.BuildAndMods, 1)
        {
            TargetCompletedAt = target
        };

        Journey? captured = null;
        _repo.When(r => r.CreateJourneyAsync(Arg.Any<Journey>(), Arg.Any<CancellationToken>()))
            .Do(ci => captured = ci.ArgAt<Journey>(0));

        var returnedJourney = JourneyTestHelpers.BuildJourney();
        returnedJourney.TargetCompletedAt = target;
        _repo.GetJourneyByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(returnedJourney);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        captured.Should().NotBeNull();
        captured!.TargetCompletedAt.Should().Be(target);
    }

    [Fact]
    public async Task Handle_WithCoverImageUrl_PersistsCoverImageUrlToEntity()
    {
        // Arrange
        const string coverUrl = "https://example.com/cover.jpg";
        var command = new CreateJourneyCommand("My Journey", null, JourneyCategory.BuildAndMods, 1)
        {
            CoverImageUrl = coverUrl
        };

        Journey? captured = null;
        _repo.When(r => r.CreateJourneyAsync(Arg.Any<Journey>(), Arg.Any<CancellationToken>()))
            .Do(ci => captured = ci.ArgAt<Journey>(0));

        var returnedJourney = JourneyTestHelpers.BuildJourney();
        returnedJourney.CoverImageUrl = coverUrl;
        _repo.GetJourneyByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(returnedJourney);

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        captured.Should().NotBeNull();
        captured!.CoverImageUrl.Should().Be(coverUrl);
    }
}
