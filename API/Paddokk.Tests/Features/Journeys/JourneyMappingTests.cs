using FluentAssertions;
using Paddokk.Core.Features.Journeys;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.Journeys;

public class JourneyMappingTests
{
    [Fact]
    public void ToJourneyDto_MapsTargetCompletedAt()
    {
        // Arrange
        var target = new DateTime(2026, 12, 31, 0, 0, 0, DateTimeKind.Utc);
        var journey = JourneyTestHelpers.BuildJourney();
        journey.TargetCompletedAt = target;

        // Act
        var dto = JourneyMapping.ToJourneyDto(journey, "user-1");

        // Assert
        dto.TargetCompletedAt.Should().Be(target);
    }

    [Fact]
    public void ToJourneyDto_MapsCoverImageUrlAsPrimaryImageUrl()
    {
        // Arrange
        const string coverUrl = "https://example.com/cover.jpg";
        var journey = JourneyTestHelpers.BuildJourney();
        journey.CoverImageUrl = coverUrl;

        // Act
        var dto = JourneyMapping.ToJourneyDto(journey, "user-1");

        // Assert
        dto.PrimaryImageUrl.Should().Be(coverUrl);
    }

    [Fact]
    public void ToJourneyDto_PrimaryImageUrl_IsNullWhenNoCoverImage()
    {
        // Arrange
        var journey = JourneyTestHelpers.BuildJourney();
        journey.CoverImageUrl = null;

        // Act
        var dto = JourneyMapping.ToJourneyDto(journey, "user-1");

        // Assert
        dto.PrimaryImageUrl.Should().BeNull();
    }

    [Fact]
    public void ToJourneyDto_MapsIsPublic()
    {
        // Arrange
        var journey = JourneyTestHelpers.BuildJourney();
        journey.IsPublic = false;

        // Act
        var dto = JourneyMapping.ToJourneyDto(journey, "user-1");

        // Assert
        dto.IsPublic.Should().BeFalse();
    }

    [Fact]
    public void ToJourneyDto_ActivityTier_IsFullThrottleWhenHighPostRate()
    {
        // Arrange
        var journey = JourneyTestHelpers.BuildJourney();
        journey.CreatedAt = DateTime.UtcNow.AddDays(-10);
        journey.Posts = Enumerable.Range(0, 5).Select(_ => new JourneyPost
        {
            AuthorId = journey.PrincipalId,
            JourneyId = journey.Id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Images = [],
            Comments = []
        }).ToList();

        // Act
        var dto = JourneyMapping.ToJourneyDto(journey);

        // Assert
        dto.ActivityTier.Should().Be(JourneyActivityTier.FullThrottle);
    }

    [Fact]
    public void ToJourneyDto_ActivityTier_IsStalledWhenNoPosts()
    {
        // Arrange
        var journey = JourneyTestHelpers.BuildJourney();
        journey.Posts = [];

        // Act
        var dto = JourneyMapping.ToJourneyDto(journey);

        // Assert
        dto.ActivityTier.Should().Be(JourneyActivityTier.Stalled);
    }

    [Fact]
    public void ToJourneyDto_ActivityTier_UsesCompletedAtForCompletedJourneys()
    {
        // Arrange: journey created 100 days ago, completed 50 days ago with 6 posts
        // postsPerDay = 6/50 = 0.12 → SlowLane (>= 0.08, < 0.17)
        var now = DateTime.UtcNow;
        var journey = JourneyTestHelpers.BuildJourney();
        journey.Status = JourneyStatus.Completed;
        journey.CreatedAt = now.AddDays(-100);
        journey.CompletedAt = now.AddDays(-50);
        journey.Posts = Enumerable.Range(0, 6).Select(_ => new JourneyPost
        {
            AuthorId = journey.PrincipalId,
            JourneyId = journey.Id,
            CreatedAt = now,
            UpdatedAt = now,
            Images = [],
            Comments = []
        }).ToList();

        // Act
        var dto = JourneyMapping.ToJourneyDto(journey);

        // Assert
        dto.ActivityTier.Should().Be(JourneyActivityTier.SlowLane);
    }
}
