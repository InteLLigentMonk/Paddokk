using API.Data;
using API.Models.DTOs;
using API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Services;

public class JourneyService : IJourneyService
{
    private readonly PaddokkDbContext _context;
    private readonly IImageService _imageService;
    private readonly ILogger<JourneyService> _logger;

    public JourneyService(
        PaddokkDbContext context,
        IImageService imageService,
        ILogger<JourneyService> logger)
    {
        _context = context;
        _imageService = imageService;
        _logger = logger;
    }

    // Journey Management Methods
    public async Task<IEnumerable<JourneyDto>> GetUserJourneysAsync(string userId, string? currentUserId = null)
    {
        var query = _context.Journeys
            .Include(j => j.User)
            .Include(j => j.UserCar)
                .ThenInclude(c => c.CarMake)
            .Include(j => j.UserCar)
                .ThenInclude(c => c.CarModel)
            .Include(j => j.UserCar)
                .ThenInclude(c => c.CarGeneration)
            .Include(j => j.Posts)
            .Include(j => j.Subscriptions)
            .Include(j => j.Likes)
            .Where(j => j.UserId == userId);

        var journeys = await query
            .OrderByDescending(j => j.Status == JourneyStatus.Active ? 1 : 0)
            .ThenByDescending(j => j.UpdatedAt)
            .ToListAsync();

        return journeys.Select(j => MapToJourneyDto(j, currentUserId));
    }

    public async Task<JourneyDto?> GetJourneyByIdAsync(int journeyId, string? currentUserId = null)
    {
        var journey = await _context.Journeys
            .Include(j => j.User)
            .Include(j => j.UserCar)
                .ThenInclude(c => c.CarMake)
            .Include(j => j.UserCar)
                .ThenInclude(c => c.CarModel)
            .Include(j => j.UserCar)
                .ThenInclude(c => c.CarGeneration)
            .Include(j => j.Posts)
            .Include(j => j.Subscriptions)
            .Include(j => j.Likes)
            .FirstOrDefaultAsync(j => j.Id == journeyId);

        return journey != null ? MapToJourneyDto(journey, currentUserId) : null;
    }

    public async Task<JourneyDto> CreateJourneyAsync(string userId, CreateJourneyRequest request)
    {
        // Validate user can create journey
        if (!await CanUserCreateJourneyAsync(userId))
            throw new InvalidOperationException("Journey limit reached for current subscription tier");

        // Validate user owns the car
        var userCar = await _context.UserCars
            .Include(c => c.CarMake)
            .Include(c => c.CarModel)
            .Include(c => c.CarGeneration)
            .FirstOrDefaultAsync(c => c.UserId == userId && c.Id == request.UserCarId && c.IsActive);

        if (userCar == null)
            throw new ArgumentException("User does not own the specified car");

        var journey = new Journey
        {
            Title = request.Title,
            Description = request.Description,
            Category = request.Category,
            Status = JourneyStatus.Active,
            UserId = userId,
            UserCarId = request.UserCarId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.Journeys.Add(journey);
        await _context.SaveChangesAsync();

        // Set as default active journey if requested
        if (request.SetAsDefaultActive)
        {
            await SetUserDefaultActiveJourneyAsync(userId, journey.Id);
        }

        _logger.LogInformation("User {UserId} created journey {JourneyId}: {Title}",
            userId, journey.Id, journey.Title);

        return await GetJourneyByIdAsync(journey.Id, userId)
            ?? throw new InvalidOperationException("Failed to retrieve created journey");
    }

    public async Task<JourneyDto?> UpdateJourneyAsync(string userId, int journeyId, UpdateJourneyRequest request)
    {
        var journey = await _context.Journeys
            .FirstOrDefaultAsync(j => j.Id == journeyId && j.UserId == userId);

        if (journey == null)
            return null;

        // Update fields
        if (!string.IsNullOrEmpty(request.Title)) journey.Title = request.Title;
        if (request.Description != null) journey.Description = request.Description;
        if (request.Category.HasValue) journey.Category = request.Category.Value;
        if (request.Status.HasValue)
        {
            journey.Status = request.Status.Value;
            if (request.Status == JourneyStatus.Completed && journey.CompletedAt == null)
            {
                journey.CompletedAt = request.CompletedAt ?? DateTime.UtcNow;
            }
        }

        journey.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} updated journey {JourneyId}", userId, journeyId);

        return await GetJourneyByIdAsync(journeyId, userId);
    }

    public async Task<bool> DeleteJourneyAsync(string userId, int journeyId)
    {
        var journey = await _context.Journeys
            .Include(j => j.Posts)
            .FirstOrDefaultAsync(j => j.Id == journeyId && j.UserId == userId);

        if (journey == null)
            return false;

        // Remove from user's default active journey if it is
        var user = await _context.Users.FindAsync(userId);
        if (user?.DefaultActiveJourneyId == journeyId)
        {
            user.DefaultActiveJourneyId = null;
            await _context.SaveChangesAsync();
        }

        // Delete all related entities
        _context.Journeys.Remove(journey);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} deleted journey {JourneyId}", userId, journeyId);

        return true;
    }

    // Journey Discovery Methods
    public async Task<IEnumerable<JourneyDto>> SearchJourneysAsync(JourneySearchRequest request, string? currentUserId = null)
    {
        var query = _context.Journeys
            .Include(j => j.User)
            .Include(j => j.UserCar)
                .ThenInclude(c => c.CarMake)
            .Include(j => j.UserCar)
                .ThenInclude(c => c.CarModel)
            .Include(j => j.UserCar)
                .ThenInclude(c => c.CarGeneration)
            .Include(j => j.Posts)
            .Include(j => j.Subscriptions)
            .Include(j => j.Likes)
            .AsQueryable();

        // Apply filters
        if (!string.IsNullOrEmpty(request.Query))
        {
            query = query.Where(j => j.Title.Contains(request.Query) ||
                                    (j.Description != null && j.Description.Contains(request.Query)));
        }

        if (request.Category.HasValue)
        {
            query = query.Where(j => j.Category == request.Category);
        }

        if (request.CarGroup.HasValue)
        {
            query = query.Where(j => j.UserCar.CarMake.Group == request.CarGroup);
        }

        if (request.CarMakeId.HasValue)
        {
            query = query.Where(j => j.UserCar.CarMakeId == request.CarMakeId);
        }

        if (request.CarModelId.HasValue)
        {
            query = query.Where(j => j.UserCar.CarModelId == request.CarModelId);
        }

        if (request.Status.HasValue)
        {
            query = query.Where(j => j.Status == request.Status);
        }

        // Apply sorting
        query = request.SortBy switch
        {
            JourneySortBy.Newest => query.OrderByDescending(j => j.CreatedAt),
            JourneySortBy.MostLiked => query.OrderByDescending(j => j.Likes.Count),
            JourneySortBy.MostSubscribed => query.OrderByDescending(j => j.Subscriptions.Count(s => s.IsActive)),
            JourneySortBy.RecentlyCompleted => query.Where(j => j.Status == JourneyStatus.Completed)
                .OrderByDescending(j => j.CompletedAt),
            _ => query.OrderByDescending(j => j.UpdatedAt) // RecentActivity
        };

        var journeys = await query
            .Skip(request.Skip)
            .Take(request.Take)
            .ToListAsync();

        return journeys.Select(j => MapToJourneyDto(j, currentUserId));
    }

    public async Task<IEnumerable<JourneyDto>> GetFeaturedJourneysAsync(string? currentUserId = null)
    {
        // For now, return most liked journeys
        var request = new JourneySearchRequest
        {
            SortBy = JourneySortBy.MostLiked,
            Take = 10
        };
        return await SearchJourneysAsync(request, currentUserId);
    }

    public async Task<IEnumerable<JourneyDto>> GetTrendingJourneysAsync(string? currentUserId = null)
    {
        // For now, return recently active journeys
        var request = new JourneySearchRequest
        {
            SortBy = JourneySortBy.RecentActivity,
            Take = 10
        };
        return await SearchJourneysAsync(request, currentUserId);
    }

    // Journey Posts Methods
    public async Task<IEnumerable<JourneyPostDto>> GetJourneyPostsAsync(int journeyId, int skip = 0, int take = 20, string? currentUserId = null)
    {
        var posts = await _context.JourneyPosts
            .Include(p => p.User)
            .Include(p => p.Images)
            .Include(p => p.Comments)
            .Where(p => p.JourneyId == journeyId)
            .OrderBy(p => p.CreatedAt) // Chronological order for journey timeline
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return posts.Select(p => MapToJourneyPostDto(p, currentUserId));
    }

    public async Task<JourneyPostDto?> GetJourneyPostByIdAsync(int postId, string? currentUserId = null)
    {
        var post = await _context.JourneyPosts
            .Include(p => p.User)
            .Include(p => p.Images)
            .Include(p => p.Comments)
            .FirstOrDefaultAsync(p => p.Id == postId);

        return post != null ? MapToJourneyPostDto(post, currentUserId) : null;
    }

    public async Task<JourneyPostDto> CreateJourneyPostAsync(string userId, int journeyId, CreateJourneyPostRequest request)
    {
        // Validate user can post to journey
        if (!await CanUserPostToJourneyAsync(userId, journeyId))
            throw new InvalidOperationException("User cannot post to this journey");

        // Validate images against subscription limits
        if (request.Images.Count != 0)
        {
            await _imageService.ValidatePostImagesAsync(userId, request.Images);
        }

        var post = new JourneyPost
        {
            JourneyId = journeyId,
            UserId = userId,
            TextContent = request.TextContent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.JourneyPosts.Add(post);
        await _context.SaveChangesAsync();

        // Add images
        if (request.Images.Any())
        {
            var images = request.Images.Select(img => new JourneyPostImage
            {
                JourneyPostId = post.Id,
                ImageUrl = img.ImageUrl,
                Caption = img.Caption,
                SortOrder = img.SortOrder,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            _context.JourneyPostImages.AddRange(images);
            await _context.SaveChangesAsync();
        }

        // Update journey's UpdatedAt
        var journey = await _context.Journeys.FindAsync(journeyId);
        if (journey != null)
        {
            journey.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

        _logger.LogInformation("User {UserId} created post {PostId} in journey {JourneyId} with {ImageCount} images",
            userId, post.Id, journeyId, request.Images.Count);

        return await GetJourneyPostByIdAsync(post.Id, userId)
            ?? throw new InvalidOperationException("Failed to retrieve created post");
    }

    public async Task<JourneyPostDto?> UpdateJourneyPostAsync(string userId, int postId, UpdateJourneyPostRequest request)
    {
        var post = await _context.JourneyPosts
            .FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);

        if (post == null)
            return null;

        if (request.TextContent != null)
        {
            post.TextContent = request.TextContent;
            post.IsEdited = true;
            post.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} updated post {PostId}", userId, postId);

        return await GetJourneyPostByIdAsync(postId, userId);
    }

    public async Task<bool> DeleteJourneyPostAsync(string userId, int postId)
    {
        var post = await _context.JourneyPosts
            .FirstOrDefaultAsync(p => p.Id == postId && p.UserId == userId);

        if (post == null)
            return false;

        _context.JourneyPosts.Remove(post);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} deleted post {PostId}", userId, postId);

        return true;
    }

    // Journey Engagement Methods
    public async Task<bool> SubscribeToJourneyAsync(string userId, int journeyId)
    {
        // Check if already subscribed
        var existing = await _context.JourneySubscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.JourneyId == journeyId);

        if (existing != null)
        {
            if (!existing.IsActive)
            {
                existing.IsActive = true;
                await _context.SaveChangesAsync();
            }
            return true;
        }

        var subscription = new JourneySubscription
        {
            UserId = userId,
            JourneyId = journeyId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };

        _context.JourneySubscriptions.Add(subscription);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} subscribed to journey {JourneyId}", userId, journeyId);

        return true;
    }

    public async Task<bool> UnsubscribeFromJourneyAsync(string userId, int journeyId)
    {
        var subscription = await _context.JourneySubscriptions
            .FirstOrDefaultAsync(s => s.UserId == userId && s.JourneyId == journeyId);

        if (subscription == null)
            return false;

        subscription.IsActive = false;
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} unsubscribed from journey {JourneyId}", userId, journeyId);

        return true;
    }

    public async Task<bool> LikeJourneyAsync(string userId, int journeyId)
    {
        // Check if already liked
        var existing = await _context.JourneyLikes
            .FirstOrDefaultAsync(l => l.UserId == userId && l.JourneyId == journeyId);

        if (existing != null)
            return true;

        var like = new JourneyLike
        {
            UserId = userId,
            JourneyId = journeyId,
            CreatedAt = DateTime.UtcNow
        };

        _context.JourneyLikes.Add(like);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} liked journey {JourneyId}", userId, journeyId);

        return true;
    }

    public async Task<bool> UnlikeJourneyAsync(string userId, int journeyId)
    {
        var like = await _context.JourneyLikes
            .FirstOrDefaultAsync(l => l.UserId == userId && l.JourneyId == journeyId);

        if (like == null)
            return false;

        _context.JourneyLikes.Remove(like);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} unliked journey {JourneyId}", userId, journeyId);

        return true;
    }

    public async Task<bool> IsSubscribedToJourneyAsync(string userId, int journeyId)
    {
        return await _context.JourneySubscriptions
            .AnyAsync(s => s.UserId == userId && s.JourneyId == journeyId && s.IsActive);
    }

    public async Task<bool> HasLikedJourneyAsync(string userId, int journeyId)
    {
        return await _context.JourneyLikes
            .AnyAsync(l => l.UserId == userId && l.JourneyId == journeyId);
    }

    // User Default Journey Methods
    public async Task<JourneyDto?> GetUserDefaultActiveJourneyAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user?.DefaultActiveJourneyId == null)
            return null;

        return await GetJourneyByIdAsync(user.DefaultActiveJourneyId.Value, userId);
    }

    public async Task<bool> SetUserDefaultActiveJourneyAsync(string userId, int journeyId)
    {
        // Verify user owns the journey
        var journey = await _context.Journeys
            .FirstOrDefaultAsync(j => j.Id == journeyId && j.UserId == userId);

        if (journey == null)
            return false;

        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        user.DefaultActiveJourneyId = journeyId;
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} set default active journey to {JourneyId}", userId, journeyId);

        return true;
    }

    // Stats Methods
    public async Task<JourneyStatsDto> GetUserJourneyStatsAsync(string userId)
    {
        var journeys = await _context.Journeys
            .Include(j => j.Posts)
            .Include(j => j.Subscriptions)
            .Include(j => j.Likes)
            .Where(j => j.UserId == userId)
            .ToListAsync();

        return new JourneyStatsDto
        {
            TotalJourneys = journeys.Count,
            ActiveJourneys = journeys.Count(j => j.Status == JourneyStatus.Active),
            CompletedJourneys = journeys.Count(j => j.Status == JourneyStatus.Completed),
            TotalPosts = journeys.Sum(j => j.Posts.Count),
            TotalSubscribers = journeys.Sum(j => j.Subscriptions.Count(s => s.IsActive)),
            TotalLikes = journeys.Sum(j => j.Likes.Count)
        };
    }

    // Validation Methods
    public async Task<bool> CanUserCreateJourneyAsync(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return false;

        var currentJourneyCount = await _context.Journeys
            .CountAsync(j => j.UserId == userId);

        var maxJourneys = user.SubscriptionTier switch
        {
            SubscriptionTier.Free => 1,
            SubscriptionTier.Silver => 3,
            SubscriptionTier.Gold => 10,
            SubscriptionTier.Platinum => 20,
            SubscriptionTier.Diamond => int.MaxValue,
            _ => 1
        };

        return currentJourneyCount < maxJourneys;
    }

    public async Task<bool> CanUserPostToJourneyAsync(string userId, int journeyId)
    {
        var journey = await _context.Journeys
            .FirstOrDefaultAsync(j => j.Id == journeyId);

        if (journey == null)
            return false;

        // User must own the journey
        return journey.UserId == userId;
    }

    // Helper Methods
    private JourneyDto MapToJourneyDto(Journey journey, string? currentUserId = null)
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
            IsSubscribed = !string.IsNullOrEmpty(currentUserId) && journey.Subscriptions.Any(s => s.UserId == currentUserId && s.IsActive),
            IsLiked = !string.IsNullOrEmpty(currentUserId) && journey.Likes.Any(l => l.UserId == currentUserId),
            IsOwner = !string.IsNullOrEmpty(currentUserId) && journey.UserId == currentUserId,

            CreatedAt = journey.CreatedAt,
            UpdatedAt = journey.UpdatedAt,
            CompletedAt = journey.CompletedAt,

            LastPostAt = lastPost?.CreatedAt,
            LastPostPreview = lastPost?.TextContent?.Length > 100
                ? lastPost.TextContent.Substring(0, 100) + "..."
                : lastPost?.TextContent
        };
    }

    private JourneyPostDto MapToJourneyPostDto(JourneyPost post, string? currentUserId = null)
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
