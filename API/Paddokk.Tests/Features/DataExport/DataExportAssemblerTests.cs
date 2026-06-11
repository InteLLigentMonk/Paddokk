using FluentAssertions;
using NSubstitute;
using Paddokk.Core.Features.DataExport;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Tests.Features.DataExport;

public class DataExportAssemblerTests
{
    private readonly IDataExportReader _reader = Substitute.For<IDataExportReader>();
    private readonly DataExportAssembler _assembler;

    public DataExportAssemblerTests()
    {
        _assembler = new DataExportAssembler(_reader);
    }

    private void SeedMinimal(string userId)
    {
        _reader.GetProfileAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new ApplicationUser { Id = userId, Username = "u", DisplayName = "U", Email = "u@example.com" });
        _reader.GetCarsWithImagesAsync(userId, Arg.Any<CancellationToken>()).Returns([]);
        _reader.GetJourneysAsync(userId, Arg.Any<CancellationToken>()).Returns([]);
        _reader.GetJourneyPostsWithImagesAsync(userId, Arg.Any<CancellationToken>()).Returns([]);
        _reader.GetCommentsAsync(userId, Arg.Any<CancellationToken>()).Returns([]);
        _reader.GetActiveFollowsAsync(userId, Arg.Any<CancellationToken>()).Returns([]);
        _reader.GetNotificationIdsAsync(userId, Arg.Any<CancellationToken>()).Returns([]);
    }

    [Fact]
    public async Task BuildAsync_PopulatesAllSectionsFromReader()
    {
        const string userId = "user-1";
        _reader.GetProfileAsync(userId, Arg.Any<CancellationToken>())
            .Returns(new ApplicationUser { Id = userId, Username = "racer", DisplayName = "Racer", Email = "racer@example.com" });
        _reader.GetCarsWithImagesAsync(userId, Arg.Any<CancellationToken>()).Returns(new[]
        {
            new UserCar
            {
                Id = 7,
                CarMake = new CarMake { Name = "Nissan" },
                CarModel = new CarModel { Name = "Skyline" },
                Year = 1999,
                Images = new List<UserCarImage> { new() { ImageUrl = "https://blob/car-7.webp" } }
            }
        });
        _reader.GetJourneysAsync(userId, Arg.Any<CancellationToken>()).Returns(new[]
        {
            new Journey { Id = 3, Title = "Rebuild", Category = JourneyCategory.Restoration, Status = JourneyStatus.Active }
        });
        _reader.GetJourneyPostsWithImagesAsync(userId, Arg.Any<CancellationToken>()).Returns(new[]
        {
            new JourneyPost
            {
                Id = 11, JourneyId = 3, TextContent = "Day one",
                Images = new List<JourneyPostImage> { new() { ImageUrl = "https://blob/post-11.webp" } }
            }
        });
        _reader.GetCommentsAsync(userId, Arg.Any<CancellationToken>()).Returns(new[]
        {
            new PostComment { Id = 21, JourneyPostId = 11, Content = "Nice" }
        });
        _reader.GetActiveFollowsAsync(userId, Arg.Any<CancellationToken>()).Returns(new[]
        {
            new UserFollow { FollowerId = userId, FollowedId = "user-2", IsActive = true }
        });
        _reader.GetNotificationIdsAsync(userId, Arg.Any<CancellationToken>()).Returns(new[] { 100, 101 });

        var doc = await _assembler.BuildAsync(userId, CancellationToken.None);

        doc.Profile.Email.Should().Be("racer@example.com");
        doc.Cars.Should().ContainSingle()
            .Which.Should().BeEquivalentTo(new { Make = "Nissan", Model = "Skyline", Year = 1999 });
        doc.Cars[0].ImageUrls.Should().ContainSingle().Which.Should().Be("https://blob/car-7.webp");
        doc.Journeys.Should().ContainSingle().Which.Title.Should().Be("Rebuild");
        doc.JourneyPosts.Should().ContainSingle();
        doc.JourneyPosts[0].ImageUrls.Should().Contain("https://blob/post-11.webp");
        doc.Comments.Should().ContainSingle().Which.Content.Should().Be("Nice");
        doc.Follows.Should().ContainSingle().Which.FollowedUserId.Should().Be("user-2");
        doc.NotificationIds.Should().BeEquivalentTo(new[] { 100, 101 });
    }

    [Fact]
    public async Task BuildAsync_ScopesEveryReaderCallToTheGivenUser()
    {
        const string userId = "user-1";
        SeedMinimal(userId);

        await _assembler.BuildAsync(userId, CancellationToken.None);

        // Cross-tenant safety: the assembler only ever asks the reader for this user's data.
        await _reader.Received(1).GetProfileAsync(userId, Arg.Any<CancellationToken>());
        await _reader.Received(1).GetCarsWithImagesAsync(userId, Arg.Any<CancellationToken>());
        await _reader.Received(1).GetJourneysAsync(userId, Arg.Any<CancellationToken>());
        await _reader.Received(1).GetJourneyPostsWithImagesAsync(userId, Arg.Any<CancellationToken>());
        await _reader.Received(1).GetCommentsAsync(userId, Arg.Any<CancellationToken>());
        await _reader.Received(1).GetActiveFollowsAsync(userId, Arg.Any<CancellationToken>());
        await _reader.Received(1).GetNotificationIdsAsync(userId, Arg.Any<CancellationToken>());
    }
}
