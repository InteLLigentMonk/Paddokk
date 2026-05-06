using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.Journeys.Commands.CreateJourneyPost;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Journeys.Commands;

public class CreateJourneyPostHandlerTests
{
    private readonly IJourneyRepository _repo = Substitute.For<IJourneyRepository>();
    private readonly IImageService _imageService = Substitute.For<IImageService>();
    private readonly IActorResolver _actor = Substitute.For<IActorResolver>();
    private readonly CreateJourneyPostHandler _handler;

    public CreateJourneyPostHandlerTests()
    {
        _actor.UserId.Returns("user-1");
        _handler = new CreateJourneyPostHandler(_repo, _imageService, _actor);
    }

    [Fact]
    public async Task Handle_WhenJourneyIsCompleted_ReturnsFailure()
    {
        // Arrange
        var journey = JourneyTestHelpers.BuildJourney();
        journey.Status = JourneyStatus.Completed;
        _repo.GetJourneyByIdAsync(1, Arg.Any<CancellationToken>()).Returns(journey);

        var command = new CreateJourneyPostCommand(1, "Some text", []);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error.Message.Should().Contain("completed");
    }

    [Fact]
    public async Task Handle_WhenJourneyIsActive_CreatesPost()
    {
        // Arrange
        var journey = JourneyTestHelpers.BuildJourney();
        journey.Status = JourneyStatus.Active;
        _repo.GetJourneyByIdAsync(1, Arg.Any<CancellationToken>()).Returns(journey);
        _repo.CreateJourneyPostAsync(Arg.Any<JourneyPost>(), Arg.Any<CancellationToken>()).Returns(1);

        var createdPost = new JourneyPost
        {
            Id = 1,
            JourneyId = 1,
            UserId = "user-1",
            TextContent = "Some text",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            User = new ApplicationUser { Id = "user-1", DisplayName = "Test User" },
            Images = [],
            Comments = []
        };
        _repo.GetJourneyPostByIdAsync(Arg.Any<int>(), Arg.Any<CancellationToken>()).Returns(createdPost);

        var command = new CreateJourneyPostCommand(1, "Some text", []);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }
}
