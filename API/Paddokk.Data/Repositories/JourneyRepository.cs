using Microsoft.EntityFrameworkCore;
using Paddokk.Core.Common.Pagination;
using Paddokk.Core.Features.Journeys.Queries.GetJourneysBrowseStats;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs.Journey;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Data.Repositories;

public class JourneyRepository : IJourneyRepository
{
    private readonly PaddokkDbContext _db;

    public JourneyRepository(PaddokkDbContext db)
    {
        _db = db;
    }

    // Journey queries
    public async Task<List<Journey>> GetUserJourneysAsync(string userId, CancellationToken cancellationToken)
    {
        return await _db.Journeys
            .AsNoTracking()
            .Include(j => j.User)
            .Include(j => j.UserCar).ThenInclude(c => c.CarMake)
            .Include(j => j.UserCar).ThenInclude(c => c.CarModel)
            .Include(j => j.UserCar).ThenInclude(c => c.CarGeneration)
            .Include(j => j.Posts)
            .Include(j => j.Subscriptions)
            .Include(j => j.Likes)
            .Where(j => j.PrincipalId == userId)
            .OrderByDescending(j => j.Status == JourneyStatus.Active ? 1 : 0)
            .ThenByDescending(j => j.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(List<Journey> Journeys, int TotalCount)> GetUserJourneysPagedAsync(string userId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var (p, s) = PaginationDefaults.Normalize(page, pageSize);

        var baseQuery = _db.Journeys.Where(j => j.PrincipalId == userId);
        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var journeys = await baseQuery
            .AsNoTracking()
            .Include(j => j.User)
            .Include(j => j.UserCar).ThenInclude(c => c.CarMake)
            .Include(j => j.UserCar).ThenInclude(c => c.CarModel)
            .Include(j => j.UserCar).ThenInclude(c => c.CarGeneration)
            .Include(j => j.Posts)
            .Include(j => j.Subscriptions)
            .Include(j => j.Likes)
            .OrderByDescending(j => j.Status == JourneyStatus.Active ? 1 : 0)
            .ThenByDescending(j => j.UpdatedAt)
            .Skip((p - 1) * s)
            .Take(s)
            .ToListAsync(cancellationToken);

        return (journeys, totalCount);
    }

    public async Task<Journey?> GetJourneyByIdAsync(int journeyId, CancellationToken cancellationToken)
    {
        return await _db.Journeys
            .Include(j => j.User)
            .Include(j => j.UserCar).ThenInclude(c => c.CarMake)
            .Include(j => j.UserCar).ThenInclude(c => c.CarModel)
            .Include(j => j.UserCar).ThenInclude(c => c.CarGeneration)
            .Include(j => j.Posts)
            .Include(j => j.Subscriptions)
            .Include(j => j.Likes)
            .FirstOrDefaultAsync(j => j.Id == journeyId, cancellationToken);
  }

    public async Task<List<Journey>> GetUserJourneysByUsernameAsync(string username, string? currentUserId, CancellationToken cancellationToken)
    {
        return await _db.Journeys
            .AsNoTracking()
            .Include(j => j.User)
            .Include(j => j.UserCar).ThenInclude(c => c.CarMake)
            .Include(j => j.UserCar).ThenInclude(c => c.CarModel)
            .Include(j => j.UserCar).ThenInclude(c => c.CarGeneration)
            .Include(j => j.Posts)
            .Include(j => j.Subscriptions)
            .Include(j => j.Likes)
            .Where(j => j.User.Username == username
                && (j.PrincipalId == currentUserId
                    || (j.IsPublic && j.UserCar.IsPublic)))
            .OrderByDescending(j => j.Status == JourneyStatus.Active ? 1 : 0)
            .ThenByDescending(j => j.UpdatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Journey>> GetCarJourneysAsync(
        string username,
        string carSlug,
        string? currentUserId,
        int page,
        int pageSize,
        CancellationToken cancellationToken)
    {
        return await _db.Journeys
            .AsNoTracking()
            .Include(j => j.User)
            .Include(j => j.UserCar).ThenInclude(c => c.CarMake)
            .Include(j => j.UserCar).ThenInclude(c => c.CarModel)
            .Include(j => j.UserCar).ThenInclude(c => c.CarGeneration)
            .Include(j => j.Posts)
            .Include(j => j.Subscriptions)
            .Include(j => j.Likes)
            .Where(j => j.User.Username == username
                && j.UserCar.Slug == carSlug
                && (j.PrincipalId == currentUserId
                    || (j.IsPublic && j.UserCar.IsPublic)))
            .OrderByDescending(j => j.Status == JourneyStatus.Active ? 1 : 0)
            .ThenByDescending(j => j.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }

    public async Task<Journey?> GetJourneyBySlugAsync(string username, string slug, string? currentUserId, CancellationToken cancellationToken)
    {
        return await _db.Journeys
            .AsNoTracking()
            .Include(j => j.User)
            .Include(j => j.UserCar).ThenInclude(c => c.CarMake)
            .Include(j => j.UserCar).ThenInclude(c => c.CarModel)
            .Include(j => j.UserCar).ThenInclude(c => c.CarGeneration)
            .Include(j => j.Posts)
            .Include(j => j.Subscriptions)
            .Include(j => j.Likes)
            .Where(j => j.User.Username == username && j.Slug == slug
                && (j.PrincipalId == currentUserId
                    || (j.IsPublic && j.UserCar.IsPublic)))
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<(List<Journey> Journeys, int Total)> SearchJourneysAsync(JourneySearchRequest request, CancellationToken cancellationToken)
    {
        var query = BuildSearchQuery(request);

        var total = await query.CountAsync(cancellationToken);

        var searchTerm = string.Join(" ", request.Terms.Where(t => !string.IsNullOrWhiteSpace(t)));

        IOrderedQueryable<Journey> ordered = request.SortBy switch
        {
            JourneySortBy.Newest => query.OrderByDescending(j => j.CreatedAt),
            JourneySortBy.MostLiked => query.OrderByDescending(j => j.Likes.Count)
                .ThenByDescending(j => j.UpdatedAt),
            JourneySortBy.MostSubscribed => query.OrderByDescending(j => j.Subscriptions.Count(s => s.IsActive))
                .ThenByDescending(j => j.UpdatedAt),
            JourneySortBy.RecentlyCompleted => query.Where(j => j.Status == JourneyStatus.Completed && j.CompletedAt != null)
                .OrderByDescending(j => j.CompletedAt),
            JourneySortBy.Relevance when !string.IsNullOrWhiteSpace(searchTerm) =>
                query.OrderBy(j => EF.Functions.TrigramsWordSimilarityDistance(searchTerm, j.SearchText!))
                    .ThenByDescending(j => j.UpdatedAt),
            _ => query.OrderByDescending(j => j.UpdatedAt)
        };

        var journeys = await ordered
            .AsNoTracking()
            .Include(j => j.User)
            .Include(j => j.UserCar).ThenInclude(c => c.CarMake)
            .Include(j => j.UserCar).ThenInclude(c => c.CarModel)
            .Include(j => j.UserCar).ThenInclude(c => c.CarGeneration)
            .Include(j => j.Posts)
            .Include(j => j.Subscriptions)
            .Include(j => j.Likes)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync(cancellationToken);

        return (journeys, total);
    }

    public async Task<GetJourneysBrowseStatsResponse> GetBrowseStatsAsync(
        JourneySearchRequest request,
        CancellationToken cancellationToken)
    {
        var query = BuildSearchQuery(request);

        var stats = await query
            .GroupBy(_ => 1)
            .Select(g => new GetJourneysBrowseStatsResponse
            {
                Journeys = g.Count(),
                Owners = g.Select(j => j.PrincipalId).Distinct().Count(),
                Posts = g.Sum(j => j.Posts.Count),
                Categories = g.Select(j => j.Category).Distinct().Count()
            })
            .FirstOrDefaultAsync(cancellationToken);

        return stats ?? new GetJourneysBrowseStatsResponse
        {
            Journeys = 0,
            Owners = 0,
            Posts = 0,
            Categories = 0
        };
    }

    public async Task<UserCar?> GetUserCarAsync(int userCarId, CancellationToken cancellationToken)
    {
        return await _db.UserCars
            .Include(c => c.CarMake)
            .Include(c => c.CarModel)
            .FirstOrDefaultAsync(c => c.Id == userCarId, cancellationToken);
    }

    private IQueryable<Journey> BuildSearchQuery(JourneySearchRequest request)
    {
        var query = _db.Journeys
            .Where(j => j.IsPublic && j.UserCar.IsPublic);

        foreach (var term in request.Terms.Where(t => !string.IsNullOrWhiteSpace(t)))
        {
            var captured = term;
            query = query.Where(j => j.SearchText != null &&
                EF.Functions.TrigramsWordSimilarity(captured, j.SearchText) >= 0.5);
        }

        if (request.Category.HasValue)
            query = query.Where(j => j.Category == request.Category);

        if (request.CarGroup.HasValue)
            query = query.Where(j => j.UserCar.CarMake != null && j.UserCar.CarMake.Group == request.CarGroup);

        if (request.CarMakeId.HasValue)
            query = query.Where(j => j.UserCar.CarMakeId == request.CarMakeId);

        if (request.CarModelId.HasValue)
            query = query.Where(j => j.UserCar.CarModelId == request.CarModelId);

        if (request.Status.HasValue)
            query = query.Where(j => j.Status == request.Status);

        return query;
    }

    public Task<int> GetUserJourneyCountAsync(string userId, CancellationToken cancellationToken)
    {
        return _db.Journeys.CountAsync(j => j.PrincipalId == userId, cancellationToken);
    }

    // Journey mutations
    public async Task<int> CreateJourneyAsync(Journey journey, CancellationToken cancellationToken)
    {
        _db.Journeys.Add(journey);
        await _db.SaveChangesAsync(cancellationToken);
        return journey.Id;
    }

    public async Task<bool> SlugExistsAsync(string principalId, string slug, CancellationToken cancellationToken)
    {
        return await _db.Journeys
            .AnyAsync(j => j.PrincipalId == principalId && j.Slug == slug, cancellationToken);
    }

    public async Task UpdateJourneyAsync(Journey journey, CancellationToken cancellationToken)
    {
        _db.Journeys.Update(journey);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteJourneyAsync(int journeyId, CancellationToken cancellationToken)
    {
        await _db.Journeys
        .Where(j => j.Id == journeyId)
        .ExecuteDeleteAsync(cancellationToken);
    }

    // Journey posts
    public async Task<(List<JourneyPost> Posts, int TotalCount)> GetJourneyPostsAsync(int journeyId, int page, int pageSize, CancellationToken cancellationToken)
    {
        var (p, s) = PaginationDefaults.Normalize(page, pageSize);

        var baseQuery = _db.JourneyPosts.Where(jp => jp.JourneyId == journeyId);
        var totalCount = await baseQuery.CountAsync(cancellationToken);

        var posts = await baseQuery
            .AsNoTracking()
            .Include(jp => jp.Author)
            .Include(jp => jp.Images)
            .Include(jp => jp.Comments)
            .OrderByDescending(jp => jp.CreatedAt)
            .Skip((p - 1) * s)
            .Take(s)
            .ToListAsync(cancellationToken);

        return (posts, totalCount);
    }

    public async Task<JourneyPost?> GetJourneyPostByIdAsync(int postId, CancellationToken cancellationToken)
    {
        return await _db.JourneyPosts
            .Include(p => p.Author)
            .Include(p => p.Images)
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == postId, cancellationToken);
    }

    public async Task<int> CreateJourneyPostAsync(JourneyPost post, CancellationToken cancellationToken)
    {
        _db.JourneyPosts.Add(post);
        await _db.SaveChangesAsync(cancellationToken);
        return post.Id;
    }

    public async Task UpdateJourneyPostAsync(JourneyPost post, CancellationToken cancellationToken)
    {
        _db.JourneyPosts.Update(post);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteJourneyPostAsync(int postId, CancellationToken cancellationToken)
    {
        await _db.JourneyPosts
            .Where(p => p.Id == postId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task AddPostImagesAsync(List<JourneyPostImage> images, CancellationToken cancellationToken)
    {
        _db.JourneyPostImages.AddRange(images);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task TouchJourneyAsync(int journeyId, CancellationToken cancellationToken)
    {
        await _db.Journeys
            .Where(j => j.Id == journeyId)
            .ExecuteUpdateAsync(j => j.SetProperty(p => p.UpdatedAt, DateTime.UtcNow), cancellationToken);
    }

    // Engagement
    public async Task<JourneySubscription?> GetSubscriptionAsync(string userId, int journeyId, CancellationToken cancellationToken)
    {
        return await _db.JourneySubscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.JourneyId == journeyId, cancellationToken);
    }

    public async Task CreateSubscriptionAsync(JourneySubscription subscription, CancellationToken cancellationToken)
    {
        _db.JourneySubscriptions.Add(subscription);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateSubscriptionAsync(JourneySubscription subscription, CancellationToken cancellationToken)
    {
        _db.JourneySubscriptions.Update(subscription);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteSubscriptionAsync(string userId, int journeyId, CancellationToken cancellationToken)
    {
        await _db.JourneySubscriptions
            .Where(s => s.UserId == userId && s.JourneyId == journeyId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task<JourneyLike?> GetLikeAsync(string userId, int journeyId, CancellationToken cancellationToken)
    {
        return await _db.JourneyLikes
            .FirstOrDefaultAsync(l => l.UserId == userId && l.JourneyId == journeyId, cancellationToken);
    }

    public async Task CreateLikeAsync(JourneyLike like, CancellationToken cancellationToken)
    {
        _db.JourneyLikes.Add(like);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteLikeAsync(string userId, int journeyId, CancellationToken cancellationToken)
    {
        await _db.JourneyLikes
            .Where(l => l.UserId == userId && l.JourneyId == journeyId)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public Task<bool> IsSubscribedAsync(string userId, int journeyId, CancellationToken cancellationToken)
    {
        return _db.JourneySubscriptions
            .AnyAsync(s => s.UserId == userId && s.JourneyId == journeyId && s.IsActive, cancellationToken);
    }

    public Task<bool> HasLikedAsync(string userId, int journeyId, CancellationToken cancellationToken)
    {
        return _db.JourneyLikes
            .AnyAsync(l => l.UserId == userId && l.JourneyId == journeyId, cancellationToken);
    }

    // User default journey
    public async Task<ApplicationUser?> GetUserAsync(string userId, CancellationToken cancellationToken)
    {
        return await _db.Users.FindAsync([userId], cancellationToken);
    }

    public async Task UpdateUserDefaultJourneyAsync(string userId, int? journeyId, CancellationToken cancellationToken)
    {
        await _db.Users
            .Where(u => u.Id == userId)
            .ExecuteUpdateAsync(u => u.SetProperty(p => p.DefaultActiveJourneyId, journeyId), cancellationToken);
    }

    // Stats
    public async Task<List<Journey>> GetUserJourneysWithStatsAsync(string userId, CancellationToken cancellationToken)
    {
        return await _db.Journeys
            .AsNoTracking()
            .Include(j => j.Posts)
            .Include(j => j.Subscriptions)
            .Include(j => j.Likes)
            .Where(j => j.PrincipalId == userId)
            .ToListAsync(cancellationToken);
    }
}
