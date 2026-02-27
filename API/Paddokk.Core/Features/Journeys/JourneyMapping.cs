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

            UserId = journey.UserId,
            UserDisplayName = journey.User.DisplayName,
            UserAvatarUrl = journey.User.AvatarUrl,

            UserCarId = journey.UserCarId,
            CarMakeName = journey.UserCar.CarMake.Name,
            CarModelName = journey.UserCar.CarModel.Name,
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
            IsOwner = !string.IsNullOrEmpty(currentUserId) && journey.UserId == currentUserId,

            CreatedAt = journey.CreatedAt,
            UpdatedAt = journey.UpdatedAt,
            CompletedAt = journey.CompletedAt,

            LastPostAt = lastPost?.CreatedAt,
            LastPostPreview = lastPost?.TextContent?.Length > 100
                ? lastPost.TextContent[..100] + "..."
                : lastPost?.TextContent
        };
    }

    internal static JourneyPostDto ToJourneyPostDto(JourneyPost post, string? currentUserId = null)
    {
        return new JourneyPostDto
        {
            Id = post.Id,
            JourneyId = post.JourneyId,
            UserId = post.UserId,
            UserDisplayName = post.User.DisplayName,
            UserAvatarUrl = post.User.AvatarUrl,
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
            IsOwner = !string.IsNullOrEmpty(currentUserId) && post.UserId == currentUserId
        };
    }
}
