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
}
