using Microsoft.EntityFrameworkCore;
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

    public async Task<List<Journey>> SearchJourneysAsync(JourneySearchRequest request, CancellationToken cancellationToken)
    {
        var query = _db.Journeys
            .Include(j => j.User)
            .Include(j => j.UserCar).ThenInclude(c => c.CarMake)
            .Include(j => j.UserCar).ThenInclude(c => c.CarModel)
            .Include(j => j.UserCar).ThenInclude(c => c.CarGeneration)
            .Include(j => j.Posts)
            .Include(j => j.Subscriptions)
            .Include(j => j.Likes)
            .Where(j => j.IsPublic && j.UserCar.IsPublic)
            .AsQueryable();

        if (!string.IsNullOrEmpty(request.Query))
            query = query.Where(j => j.Title.Contains(request.Query) ||
               (j.Description != null && j.Description.Contains(request.Query)));

        if (request.Category.HasValue)
            query = query.Where(j => j.Category == request.Category);

        if (request.CarGroup.HasValue)
            query = query.Where(j => j.UserCar.CarMake.Group == request.CarGroup);

        if (request.CarMakeId.HasValue)
            query = query.Where(j => j.UserCar.CarMakeId == request.CarMakeId);

        if (request.CarModelId.HasValue)
            query = query.Where(j => j.UserCar.CarModelId == request.CarModelId);

        if (request.Status.HasValue)
            query = query.Where(j => j.Status == request.Status);

        query = request.SortBy switch
        {
            JourneySortBy.Newest => query.OrderByDescending(j => j.CreatedAt),
            JourneySortBy.MostLiked => query.OrderByDescending(j => j.Likes.Count),
            JourneySortBy.MostSubscribed => query.OrderByDescending(j => j.Subscriptions.Count(s => s.IsActive)),
            JourneySortBy.RecentlyCompleted => query.Where(j => j.Status == JourneyStatus.Completed)
            .OrderByDescending(j => j.CompletedAt),
            _ => query.OrderByDescending(j => j.UpdatedAt)
        };

        return await query
            .Skip(request.Skip)
            .Take(request.Take)
            .ToListAsync(cancellationToken);
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
    public async Task<List<JourneyPost>> GetJourneyPostsAsync(int journeyId, int skip, int take, CancellationToken cancellationToken)
    {
        return await _db.JourneyPosts
            .Include(p => p.Author)
            .Include(p => p.Images)
            .Include(p => p.Comments)
            .Where(p => p.JourneyId == journeyId)
            .OrderByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync(cancellationToken);
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
            .Include(j => j.Posts)
            .Include(j => j.Subscriptions)
            .Include(j => j.Likes)
            .Where(j => j.PrincipalId == userId)
            .ToListAsync(cancellationToken);
    }
}
