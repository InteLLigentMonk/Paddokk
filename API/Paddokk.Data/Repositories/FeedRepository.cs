using Microsoft.EntityFrameworkCore;
using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Feed;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Repositories;

/// <summary>
/// Composes the personalised, strictly chronological Feed directly from the underlying
/// entities — the module owns no storage of its own (#185).
///
/// Each item type is a thin UNION arm derived from an existing entity's timestamp; there
/// is no event-store table. The arms are projected to a common scalar shape so Postgres can
/// UNION them, ordered and paged across the whole set. Per-row collections (a JourneyPost's
/// images and comment count) are deliberately NOT projected inside the UNION — EF Core cannot
/// translate set operations over collection projections — so they are backfilled in a second,
/// id-scoped query against only the page that survived pagination.
///
/// Source paths by item type:
///   JourneyPost      Followed author OR Subscribed Journey OR Subscribed UserCar (Model A)
///   UserCarCreated   author Followed                                            (#186)
///   JourneyStarted   author Followed                                            (#186)
///   JourneyCompleted author Followed                                            (#186)
///
/// Per ADR-0001 this repository is not unit-tested; the UNION/projection semantics ride on
/// the SQL and are verified by inspection.
/// </summary>
public class FeedRepository(PaddokkDbContext db) : IFeedRepository
{
    public async Task<(IReadOnlyList<FeedItemDto> Items, int TotalCount)> GetFeedAsync(
        string actorId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var (p, s) = PaginationDefaults.Normalize(page, pageSize);

        // Relationship membership stays as subqueries — Postgres does the joins; we never
        // materialise the actor's follow/subscription graph into the application layer.
        var followedAuthorIds = db.UserFollows
            .Where(f => f.FollowerId == actorId && f.IsActive)
            .Select(f => f.FollowedId);
        var subscribedJourneyIds = db.JourneySubscriptions
            .Where(sub => sub.UserId == actorId && sub.IsActive)
            .Select(sub => sub.JourneyId);
        var subscribedCarIds = db.UserCarSubscriptions
            .Where(sub => sub.UserId == actorId && sub.IsActive)
            .Select(sub => sub.UserCarId);

        // Arm 1 — JourneyPost. Images and comment count are backfilled after paging (see
        // class remarks); the UNION projection carries only scalars.
        var journeyPostItems = db.JourneyPosts
            .Where(post => post.Journey.IsPublic && !post.Author.IsDeleted)
            .Where(post =>
                followedAuthorIds.Contains(post.AuthorId)
                || subscribedJourneyIds.Contains(post.JourneyId)
                || subscribedCarIds.Contains(post.Journey.UserCarId))
            .Select(post => new FeedItemDto
            {
                Type = FeedItemType.JourneyPost,
                CreatedAt = post.CreatedAt,
                ActorUsername = post.Author.Username,
                ActorDisplayName = post.Author.DisplayName,
                ActorAvatarUrl = post.Author.AvatarUrl,
                JourneyId = post.JourneyId,
                JourneyTitle = post.Journey.Title,
                JourneySlug = post.Journey.Slug,
                UserCarId = post.Journey.UserCarId,
                UserCarSlug = post.Journey.UserCar.Slug,
                UserCarLabel =
                    post.Journey.UserCar.Nickname
                    ?? post.Journey.UserCar.CustomBuildName
                    ?? (post.Journey.UserCar.CarModel != null ? post.Journey.UserCar.CarModel.Name : null),
                JourneyPostId = post.Id,
                TextContent = post.TextContent,
                ImageUrls = null,
                CommentCount = null
            });

        // Arm 2 — UserCarCreated. A Followed User adds a new UserCar.
        var userCarCreatedItems = db.UserCars
            .Where(car => car.IsActive && car.IsPublic && !car.User.IsDeleted)
            .Where(car => followedAuthorIds.Contains(car.PrincipalId))
            .Select(car => new FeedItemDto
            {
                Type = FeedItemType.UserCarCreated,
                CreatedAt = car.CreatedAt,
                ActorUsername = car.User.Username,
                ActorDisplayName = car.User.DisplayName,
                ActorAvatarUrl = car.User.AvatarUrl,
                JourneyId = null,
                JourneyTitle = null,
                JourneySlug = null,
                UserCarId = car.Id,
                UserCarSlug = car.Slug,
                UserCarLabel =
                    car.Nickname
                    ?? car.CustomBuildName
                    ?? (car.CarModel != null ? car.CarModel.Name : null),
                JourneyPostId = null,
                TextContent = null,
                ImageUrls = null,
                CommentCount = null
            });

        // Arm 3 — JourneyStarted. A Followed User starts a new Journey.
        var journeyStartedItems = db.Journeys
            .Where(journey => journey.IsPublic && !journey.User.IsDeleted)
            .Where(journey => followedAuthorIds.Contains(journey.PrincipalId))
            .Select(journey => new FeedItemDto
            {
                Type = FeedItemType.JourneyStarted,
                CreatedAt = journey.CreatedAt,
                ActorUsername = journey.User.Username,
                ActorDisplayName = journey.User.DisplayName,
                ActorAvatarUrl = journey.User.AvatarUrl,
                JourneyId = journey.Id,
                JourneyTitle = journey.Title,
                JourneySlug = journey.Slug,
                UserCarId = journey.UserCarId,
                UserCarSlug = journey.UserCar.Slug,
                UserCarLabel =
                    journey.UserCar.Nickname
                    ?? journey.UserCar.CustomBuildName
                    ?? (journey.UserCar.CarModel != null ? journey.UserCar.CarModel.Name : null),
                JourneyPostId = null,
                TextContent = null,
                ImageUrls = null,
                CommentCount = null
            });

        // Arm 4 — JourneyCompleted. A Followed User marks a Journey complete; ordered by the
        // completion moment, not the journey's creation.
        var journeyCompletedItems = db.Journeys
            .Where(journey => journey.IsPublic && !journey.User.IsDeleted)
            .Where(journey => journey.Status == JourneyStatus.Completed && journey.CompletedAt != null)
            .Where(journey => followedAuthorIds.Contains(journey.PrincipalId))
            .Select(journey => new FeedItemDto
            {
                Type = FeedItemType.JourneyCompleted,
                CreatedAt = journey.CompletedAt!.Value,
                ActorUsername = journey.User.Username,
                ActorDisplayName = journey.User.DisplayName,
                ActorAvatarUrl = journey.User.AvatarUrl,
                JourneyId = journey.Id,
                JourneyTitle = journey.Title,
                JourneySlug = journey.Slug,
                UserCarId = journey.UserCarId,
                UserCarSlug = journey.UserCar.Slug,
                UserCarLabel =
                    journey.UserCar.Nickname
                    ?? journey.UserCar.CustomBuildName
                    ?? (journey.UserCar.CarModel != null ? journey.UserCar.CarModel.Name : null),
                JourneyPostId = null,
                TextContent = null,
                ImageUrls = null,
                CommentCount = null
            });

        var feed = journeyPostItems
            .Union(userCarCreatedItems)
            .Union(journeyStartedItems)
            .Union(journeyCompletedItems);

        var totalCount = await feed.CountAsync(cancellationToken);

        var items = await feed
            // Strict chronological order; JourneyPostId is a stable tiebreaker so equal
            // timestamps do not shuffle rows across page boundaries.
            .OrderByDescending(item => item.CreatedAt)
            .ThenByDescending(item => item.JourneyPostId)
            .Skip((p - 1) * s)
            .Take(s)
            .ToListAsync(cancellationToken);

        return (await BackfillJourneyPostContentAsync(items, cancellationToken), totalCount);
    }

    /// <summary>
    /// Attaches the image strip and comment count to the JourneyPost rows on the current
    /// page. Scoped to the page's post ids so the collection load never fans out across the
    /// whole feed, and kept out of the UNION where EF cannot translate it.
    /// </summary>
    private async Task<IReadOnlyList<FeedItemDto>> BackfillJourneyPostContentAsync(
        List<FeedItemDto> items,
        CancellationToken cancellationToken)
    {
        var postIds = items
            .Where(item => item.Type == FeedItemType.JourneyPost && item.JourneyPostId != null)
            .Select(item => item.JourneyPostId!.Value)
            .ToList();

        if (postIds.Count == 0)
            return items;

        var content = await db.JourneyPosts
            .Where(post => postIds.Contains(post.Id))
            .Select(post => new
            {
                post.Id,
                ImageUrls = post.Images
                    .OrderBy(img => img.SortOrder)
                    .Select(img => img.ImageUrl)
                    .ToList(),
                CommentCount = post.Comments.Count
            })
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        return items
            .Select(item =>
                item.Type == FeedItemType.JourneyPost
                && item.JourneyPostId is int id
                && content.TryGetValue(id, out var extra)
                    ? item with { ImageUrls = extra.ImageUrls, CommentCount = extra.CommentCount }
                    : item)
            .ToList();
    }
}
