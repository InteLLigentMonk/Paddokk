using Paddokk.Core.Models.DTOs.Feed;

namespace Paddokk.Core.Interfaces;

/// <summary>
/// Read seam for the personalised Feed. The entire behaviour — the SQL UNION across the
/// JourneyPost source paths, strict <c>CreatedAt DESC</c> ordering, dedup, visibility
/// filtering, and projection to <see cref="FeedItemDto"/> — lives in the implementation.
///
/// Per ADR-0001 this interface is the handler-test seam; per ADR-0001's own consequences,
/// the projection/UNION semantics are not exercised through the mock and are verified
/// (if at all) by a separate suite. None exists yet — see #185.
/// </summary>
public interface IFeedRepository
{
    /// <summary>
    /// Returns the page of Feed items for <paramref name="actorId"/> together with the
    /// total count across the whole union, for pagination math.
    /// </summary>
    Task<(IReadOnlyList<FeedItemDto> Items, int TotalCount)> GetFeedAsync(
        string actorId,
        int page,
        int pageSize,
        CancellationToken cancellationToken);
}
