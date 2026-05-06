using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Journeys.Commands.UpdateJourney;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Journeys.Commands;

public class UpdateJourneyHandlerTests
{
    private readonly IJourneyRepository _repo = Substitute.For<IJourneyRepository>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly IHtmlSanitizationService _htmlSanitizer = Substitute.For<IHtmlSanitizationService>();
    private readonly UpdateJourneyHandler _handler;

    public UpdateJourneyHandlerTests()
    {
        _actor.UserId.Returns("user-1");
        _htmlSanitizer.Sanitize(Arg.Any<string?>()).Returns(ci => ci.ArgAt<string?>(0));
        _handler = new UpdateJourneyHandler(_repo, _actor, _htmlSanitizer);
    }

    [Fact]
    public async Task Handle_WithTargetCompletedAt_UpdatesTargetCompletedAtOnEntity()
    {
        // Arrange
        var target = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc);
        var existing = JourneyTestHelpers.BuildJourney();
        _repo.GetJourneyByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(existing, existing);

        Journey? updated = null;
        _repo.When(r => r.UpdateJourneyAsync(Arg.Any<Journey>(), Arg.Any<CancellationToken>()))
            .Do(ci => updated = ci.ArgAt<Journey>(0));

        var command = new UpdateJourneyCommand(1, null, null, null, null, null)
        {
            TargetCompletedAt = target
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        updated.Should().NotBeNull();
        updated!.TargetCompletedAt.Should().Be(target);
    }

    [Fact]
    public async Task Handle_WithCoverImageUrl_UpdatesCoverImageUrlOnEntity()
    {
        // Arrange
        const string coverUrl = "https://example.com/new-cover.jpg";
        var existing = JourneyTestHelpers.BuildJourney();
        _repo.GetJourneyByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(existing, existing);

        Journey? updated = null;
        _repo.When(r => r.UpdateJourneyAsync(Arg.Any<Journey>(), Arg.Any<CancellationToken>()))
            .Do(ci => updated = ci.ArgAt<Journey>(0));

        var command = new UpdateJourneyCommand(1, null, null, null, null, null)
        {
            CoverImageUrl = coverUrl
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        updated.Should().NotBeNull();
        updated!.CoverImageUrl.Should().Be(coverUrl);
    }

    [Fact]
    public async Task Handle_WithIsPublicFalse_SetsIsPublicFalseOnEntity()
    {
        // Arrange
        var existing = JourneyTestHelpers.BuildJourney();
        _repo.GetJourneyByIdAsync(1, Arg.Any<CancellationToken>())
            .Returns(existing, existing);

        Journey? updated = null;
        _repo.When(r => r.UpdateJourneyAsync(Arg.Any<Journey>(), Arg.Any<CancellationToken>()))
            .Do(ci => updated = ci.ArgAt<Journey>(0));

        var command = new UpdateJourneyCommand(1, null, null, null, null, null)
        {
            IsPublic = false
        };

        // Act
        await _handler.Handle(command, CancellationToken.None);

        // Assert
        updated.Should().NotBeNull();
        updated!.IsPublic.Should().BeFalse();
    }
}
