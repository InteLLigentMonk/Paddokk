using Paddokk.Core.Models.DTOs.Journey;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Journeys;

internal static class JourneyMapping
{
    internal static JourneyDto ToJourneyDto(Journey journey, string? currentUserId = null)
    {
        var lastPost = journey.Posts.OrderByDescending(p => p.CreatedAt).FirstOrDefault();

        return new JourneyDto
        {
            Id = journey.Id,
            Title = journey.Title,
            Description = journey.Description,
            Category = journey.Category,
            Status = journey.Status,
            ActivityTier = ComputeActivityTier(journey),
            IsPublic = journey.IsPublic,

            PrincipalId = journey.PrincipalId,
            UserDisplayName = journey.User.DisplayName,
            UserAvatarUrl = journey.User.AvatarUrl,

            UserCarId = journey.UserCarId,
            CarMakeName = journey.UserCar.CarMake?.Name,
            CarModelName = journey.UserCar.CarModel?.Name,
            CarGenerationName = journey.UserCar.CarGeneration?.Name,
            CarYear = journey.UserCar.Year,
            CarNickname = journey.UserCar.Nickname,
            CarPrimaryImageUrl = journey.UserCar.PrimaryImageUrl,

            PostCount = journey.Posts.Count,
            SubscriberCount = journey.Subscriptions.Count(s => s.IsActive),
            LikeCount = journey.Likes.Count,
            IsSubscribed = !string.IsNullOrEmpty(currentUserId) &&
                journey.Subscriptions.Any(s => s.UserId == currentUserId && s.IsActive),
            IsLiked = !string.IsNullOrEmpty(currentUserId) &&
                journey.Likes.Any(l => l.UserId == currentUserId),
            IsOwner = !string.IsNullOrEmpty(currentUserId) && journey.PrincipalId == currentUserId,

            PrimaryImageUrl = journey.CoverImageUrl,

            CreatedAt = journey.CreatedAt,
            UpdatedAt = journey.UpdatedAt,
            CompletedAt = journey.CompletedAt,
            TargetCompletedAt = journey.TargetCompletedAt,

            LastPostAt = lastPost?.CreatedAt,
            LastPostPreview = lastPost?.TextContent?.Length > 100
                ? lastPost.TextContent[..100] + "..."
                : lastPost?.TextContent
        };
    }

    private static JourneyActivityTier ComputeActivityTier(Journey journey)
    {
        var endDate = journey.CompletedAt ?? DateTime.UtcNow;
        var totalDays = Math.Max(1, (endDate - journey.CreatedAt).TotalDays);
        var postsPerDay = journey.Posts.Count / totalDays;

        return postsPerDay switch
        {
            >= 0.35 => JourneyActivityTier.FullThrottle,
            >= 0.17 => JourneyActivityTier.Cruising,
            >= 0.08 => JourneyActivityTier.SlowLane,
            >= 0.02 => JourneyActivityTier.Crawling,
            _ => JourneyActivityTier.Stalled
        };
    }

    internal static JourneyPostDto ToJourneyPostDto(JourneyPost post, string? currentUserId = null)
    {
        return new JourneyPostDto
        {
            Id = post.Id,
            JourneyId = post.JourneyId,
            AuthorId = post.AuthorId,
            UserDisplayName = post.Author.DisplayName,
            UserAvatarUrl = post.Author.AvatarUrl,
            TextContent = post.TextContent,
            Images = post.Images.OrderBy(i => i.SortOrder).Select(i => new JourneyPostImageDto
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl,
                Caption = i.Caption,
                SortOrder = i.SortOrder
            }).ToList(),
            CommentCount = post.Comments.Count,
            CreatedAt = post.CreatedAt,
            UpdatedAt = post.UpdatedAt,
            IsEdited = post.IsEdited,
            IsOwner = !string.IsNullOrEmpty(currentUserId) && post.AuthorId == currentUserId
        };
    }
}
