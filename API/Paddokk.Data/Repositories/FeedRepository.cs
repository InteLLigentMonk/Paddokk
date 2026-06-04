using Microsoft.EntityFrameworkCore;
using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Feed;

namespace Paddokk.Data.Repositories;

/// <summary>
/// Composes the personalised, strictly chronological Feed directly from the underlying
/// entities — the module owns no storage of its own (#185).
///
/// This tracer slice projects a single item type, <see cref="FeedItemType.JourneyPost"/>,
/// drawn from its three source paths (Followed author / Subscribed Journey / Subscribed
/// UserCar). Because all three are membership tests against the same JourneyPosts table,
/// they collapse into one projected query whose rows are inherently distinct — a post
/// matching several paths is still one row. Later slices add the other five item types as
/// additional projected arms UNION-ed onto this one (heterogeneous tables, where the UNION
/// genuinely earns its keep), ordered and paged across the whole set.
///
/// Per ADR-0001 this repository is not unit-tested; the UNION/projection semantics ride on
/// the SQL and are verified by inspection / a future suite.
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

        var journeyPostItems = db.JourneyPosts
            .AsNoTracking()
            // Visibility: never surface non-public Journeys or soft-deleted authors' content.
            .Where(post => post.Journey.IsPublic && !post.Author.IsDeleted)
            // The three JourneyPost source paths (Model A: a UserCar-Subscription covers
            // every Journey on that UserCar).
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
                ImageUrls = post.Images
                    .OrderBy(img => img.SortOrder)
                    .Select(img => img.ImageUrl)
                    .ToList(),
                CommentCount = post.Comments.Count
            });

        // The full union across all six item types is assembled here in later slices:
        //   feed = journeyPostItems.Union(lifecycleItems).Union(contentUpdateItems);
        var feed = journeyPostItems;

        var totalCount = await feed.CountAsync(cancellationToken);

        var items = await feed
            // Strict chronological order; JourneyPostId is a stable tiebreaker so equal
            // timestamps do not shuffle rows across page boundaries.
            .OrderByDescending(item => item.CreatedAt)
            .ThenByDescending(item => item.JourneyPostId)
            .Skip((p - 1) * s)
            .Take(s)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
