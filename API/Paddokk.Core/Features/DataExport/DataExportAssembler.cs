using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.DataExport;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.DataExport;

/// <summary>
/// Maps the user's rows (fetched through <see cref="IDataExportReader"/>, all scoped to the same
/// user id) into a cycle-free <see cref="DataExportDocument"/> ready for JSON serialization.
/// </summary>
public sealed class DataExportAssembler(IDataExportReader reader) : IDataExportAssembler
{
    public async Task<DataExportDocument> BuildAsync(string userId, CancellationToken ct)
    {
        var profile = await reader.GetProfileAsync(userId, ct);
        var cars = await reader.GetCarsWithImagesAsync(userId, ct);
        var journeys = await reader.GetJourneysAsync(userId, ct);
        var posts = await reader.GetJourneyPostsWithImagesAsync(userId, ct);
        var comments = await reader.GetCommentsAsync(userId, ct);
        var follows = await reader.GetActiveFollowsAsync(userId, ct);
        var notificationIds = await reader.GetNotificationIdsAsync(userId, ct);

        return new DataExportDocument(
            GeneratedAt: DateTime.UtcNow,
            Profile: MapProfile(profile, userId),
            Cars: [.. cars.Select(MapCar)],
            Journeys: [.. journeys.Select(MapJourney)],
            JourneyPosts: [.. posts.Select(MapPost)],
            Comments: [.. comments.Select(MapComment)],
            Follows: [.. follows.Select(MapFollow)],
            NotificationIds: notificationIds);
    }

    private static DataExportProfile MapProfile(ApplicationUser? user, string userId) => new(
        Id: user?.Id ?? userId,
        Username: user?.Username ?? string.Empty,
        DisplayName: user?.DisplayName ?? string.Empty,
        FirstName: user?.FirstName ?? string.Empty,
        LastName: user?.LastName,
        Email: user?.Email,
        Bio: user?.Bio,
        AvatarUrl: user?.AvatarUrl,
        CreatedAt: user?.CreatedAt ?? default);

    private static DataExportCar MapCar(UserCar car) => new(
        Id: car.Id,
        Make: car.CarMake?.Name,
        Model: car.CarModel?.Name,
        Generation: car.CarGeneration?.Name,
        Year: car.Year,
        CustomBuildName: car.CustomBuildName,
        Nickname: car.Nickname,
        Color: car.Color,
        Engine: car.Engine,
        OwnerNote: car.OwnerNote,
        CreatedAt: car.CreatedAt,
        ImageUrls: [.. car.Images.OrderBy(i => i.SortOrder).Select(i => i.ImageUrl)]);

    private static DataExportJourney MapJourney(Journey journey) => new(
        Id: journey.Id,
        Title: journey.Title,
        Description: journey.Description,
        Category: journey.Category.ToString(),
        Status: journey.Status.ToString(),
        CreatedAt: journey.CreatedAt,
        CompletedAt: journey.CompletedAt);

    private static DataExportJourneyPost MapPost(JourneyPost post) => new(
        Id: post.Id,
        JourneyId: post.JourneyId,
        TextContent: post.TextContent,
        CreatedAt: post.CreatedAt,
        ImageUrls: [.. post.Images.OrderBy(i => i.SortOrder).Select(i => i.ImageUrl)]);

    private static DataExportComment MapComment(PostComment comment) => new(
        Id: comment.Id,
        JourneyPostId: comment.JourneyPostId,
        Content: comment.Content,
        CreatedAt: comment.CreatedAt);

    private static DataExportFollow MapFollow(UserFollow follow) => new(
        FollowedUserId: follow.FollowedId,
        CreatedAt: follow.CreatedAt,
        IsActive: follow.IsActive);
}
