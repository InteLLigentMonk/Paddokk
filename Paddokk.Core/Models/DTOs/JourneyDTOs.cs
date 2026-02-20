using Paddokk.Core.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace Paddokk.Core.Models.DTOs;

public class JourneyDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public JourneyCategory Category { get; set; }
    public JourneyStatus Status { get; set; }

    // User Info
    public string UserId { get; set; } = string.Empty;
    public string UserDisplayName { get; set; } = string.Empty;
    public string? UserAvatarUrl { get; set; }

    // Car Info
    public int UserCarId { get; set; }
    public string CarMakeName { get; set; } = string.Empty;
    public string CarModelName { get; set; } = string.Empty;
    public string? CarGenerationName { get; set; }
    public int CarYear { get; set; }
    public string? CarNickname { get; set; }
    public string? CarPrimaryImageUrl { get; set; }

    // Journey Stats
    public int PostCount { get; set; }
    public int SubscriberCount { get; set; }
    public int LikeCount { get; set; }
    public bool IsSubscribed { get; set; } // For current user
    public bool IsLiked { get; set; } // For current user
    public bool IsOwner { get; set; } // For current user

    // Metadata
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }

    // Latest Post Preview
    public DateTime? LastPostAt { get; set; }
    public string? LastPostPreview { get; set; }
}

public class CreateJourneyRequest
{
    [Required]
    [StringLength(200, MinimumLength = 3)]
    public string Title { get; set; } = string.Empty;

    [StringLength(1000)]
    public string? Description { get; set; }

    [Required]
    public JourneyCategory Category { get; set; }

    [Required]
    public int UserCarId { get; set; }

    public bool SetAsDefaultActive { get; set; } = true;
}

public class UpdateJourneyRequest
{
    [StringLength(200, MinimumLength = 3)]
    public string? Title { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

    public JourneyCategory? Category { get; set; }
    public JourneyStatus? Status { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class JourneyPostDto
{
    public int Id { get; set; }
    public int JourneyId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string UserDisplayName { get; set; } = string.Empty;
    public string? UserAvatarUrl { get; set; }

    public string? TextContent { get; set; }
    public List<JourneyPostImageDto> Images { get; set; } = new();
    public int CommentCount { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsEdited { get; set; }
    public bool IsOwner { get; set; } // For current user
}

public class JourneyPostImageDto
{
    public int Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public int SortOrder { get; set; }
}

public class CreateJourneyPostRequest
{
    [StringLength(5000)]
    public string? TextContent { get; set; }

    public List<CreateJourneyPostImageRequest> Images { get; set; } = new();
}

public class CreateJourneyPostImageRequest
{
    [Required]
    public string ImageUrl { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Caption { get; set; }

    public int SortOrder { get; set; }
}

public class UpdateJourneyPostRequest
{
    [StringLength(5000)]
    public string? TextContent { get; set; }
}

public class JourneySearchRequest
{
    public string? Query { get; set; }
    public JourneyCategory? Category { get; set; }
    public CarGroup? CarGroup { get; set; }
    public int? CarMakeId { get; set; }
    public int? CarModelId { get; set; }
    public JourneyStatus? Status { get; set; }
    public JourneySortBy SortBy { get; set; } = JourneySortBy.RecentActivity;
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 20;
}

public enum JourneySortBy
{
    RecentActivity = 1,
    Newest = 2,
    MostLiked = 3,
    MostSubscribed = 4,
    RecentlyCompleted = 5
}

public class JourneyStatsDto
{
    public int TotalJourneys { get; set; }
    public int ActiveJourneys { get; set; }
    public int CompletedJourneys { get; set; }
    public int TotalPosts { get; set; }
    public int TotalSubscribers { get; set; }
    public int TotalLikes { get; set; }
}
