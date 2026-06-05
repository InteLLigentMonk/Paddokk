namespace Paddokk.Core.Models.DTOs.Feed;

/// <summary>
/// Closed set of feed item shapes. The discriminator on <see cref="FeedItemDto"/>.
/// Only <see cref="JourneyPost"/> is projected today; the remaining five land in later
/// slices as additional UNION arms, but the enum is closed and explicit from day one.
/// </summary>
public enum FeedItemType
{
    JourneyPost = 1,
    UserCarCreated = 2,
    JourneyStarted = 3,
    JourneyCompleted = 4,
    PhotosAdded = 5,
    SpecChanged = 6
}

/// <summary>
/// A single item in a User's personalised, strictly chronological Feed.
///
/// Deliberately a flat, monomorphic record discriminated by <see cref="Type"/> with
/// nullable type-specific fields, rather than a polymorphic/oneOf payload. Per ADR-0005,
/// polymorphic DTOs generate fragile Orval + Zod output; a flat shape keeps the generated
/// client a single clean type that the frontend narrows on <see cref="Type"/>.
/// </summary>
public record FeedItemDto
{
    public required FeedItemType Type { get; init; }

    /// <summary>The sort key for the whole union — strict <c>CreatedAt DESC</c>.</summary>
    public required DateTime CreatedAt { get; init; }

    // Actor — the User whose action produced this item. Always present.
    public required string ActorUsername { get; init; }
    public required string ActorDisplayName { get; init; }
    public string? ActorAvatarUrl { get; init; }

    // Journey context — present for JourneyPost and the Journey lifecycle types.
    public int? JourneyId { get; init; }
    public string? JourneyTitle { get; init; }
    public string? JourneySlug { get; init; }

    // UserCar context — present for JourneyPost and the UserCar-scoped types.
    // UserCarLabel is precomputed in the projection (Nickname / CustomBuildName / make+model).
    public int? UserCarId { get; init; }
    public string? UserCarSlug { get; init; }
    public string? UserCarLabel { get; init; }

    // JourneyPost payload — populated when Type == JourneyPost.
    public int? JourneyPostId { get; init; }
    public string? TextContent { get; init; }
    public IReadOnlyList<string>? ImageUrls { get; init; }
    public int? CommentCount { get; init; }

    // PhotosAdded payload — populated when Type == PhotosAdded. ImageUrls carries a small
    // thumbnail selection; PhotoCount is the full size of the upload session, which may
    // exceed the number of thumbnails shown.
    public int? PhotoCount { get; init; }
}
