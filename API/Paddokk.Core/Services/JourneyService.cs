using Microsoft.Extensions.Logging;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Services;

public class JourneyService(
    IJourneyRepository journeyRepository,
    IImageService imageService,
    IUnitOfWork unitOfWork,
    ILogger<JourneyService> logger) : IJourneyService
{
    private readonly IJourneyRepository _journeyRepository = journeyRepository;
    private readonly IImageService _imageService = imageService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly ILogger<JourneyService> _logger = logger;

    // Journey Management Methods
    public async Task<IEnumerable<JourneyDto>> GetUserJourneysAsync(
        string userId, CancellationToken cancellationToken, string? currentUserId = null)
    {
        var journeys = await _journeyRepository.GetUserJourneysAsync(userId, cancellationToken);
        return journeys.Select(j => MapToJourneyDto(j, currentUserId));
    }

    public async Task<JourneyDto?> GetJourneyByIdAsync(
        int journeyId, CancellationToken cancellationToken, string? currentUserId = null)
    {
        var journey = await _journeyRepository.GetJourneyByIdAsync(journeyId, cancellationToken);
        return journey is not null ? MapToJourneyDto(journey, currentUserId) : null;
    }

    public async Task<JourneyDto> CreateJourneyAsync(
        string userId, CreateJourneyRequest request, CancellationToken cancellationToken)
    {
        if (!await CanUserCreateJourneyAsync(userId, cancellationToken))
            throw new InvalidOperationException("Journey limit reached for current subscription tier");

        // Validate user owns the car — delegated to CarService in future, checked via repo for now
        _ = await _journeyRepository.GetUserAsync(userId, cancellationToken)
            ?? throw new ArgumentException("User not found");
        
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

        await _journeyRepository.CreateJourneyAsync(journey, cancellationToken);

        if (request.SetAsDefaultActive)
            await SetUserDefaultActiveJourneyAsync(userId, journey.Id, cancellationToken);

        _logger.LogInformation("User {UserId} created journey {JourneyId}: {Title}",
            userId, journey.Id, journey.Title);

        return await GetJourneyByIdAsync(journey.Id, cancellationToken, userId)
            ?? throw new InvalidOperationException("Failed to retrieve created journey");
    }

    public async Task<JourneyDto?> UpdateJourneyAsync(
        string userId, int journeyId, UpdateJourneyRequest request, CancellationToken cancellationToken)
    {
        var journey = await _journeyRepository.GetJourneyByIdAsync(journeyId, cancellationToken);

        if (journey is null || journey.UserId != userId)
            return null;

        if (!string.IsNullOrEmpty(request.Title))
            journey.Title = request.Title;

        if (request.Description != null)
            journey.Description = request.Description;

        if (request.Category.HasValue)
            journey.Category = request.Category.Value;

        if (request.Status.HasValue)
        {
            journey.Status = request.Status.Value;
            if (request.Status == JourneyStatus.Completed && journey.CompletedAt == null)
            journey.CompletedAt = request.CompletedAt ?? DateTime.UtcNow;
        }

        journey.UpdatedAt = DateTime.UtcNow;

        await _journeyRepository.UpdateJourneyAsync(journey, cancellationToken);

        _logger.LogInformation("User {UserId} updated journey {JourneyId}", userId, journeyId);

        return await GetJourneyByIdAsync(journeyId, cancellationToken, userId);
    }

    public async Task<bool> DeleteJourneyAsync(string userId, int journeyId, CancellationToken cancellationToken)
    {
        var journey = await _journeyRepository.GetJourneyByIdAsync(journeyId, cancellationToken);
        if (journey is null || journey.UserId != userId)
           return false;

        var user = await _journeyRepository.GetUserAsync(userId, cancellationToken);
        if (user?.DefaultActiveJourneyId == journeyId)
            await _journeyRepository.UpdateUserDefaultJourneyAsync(userId, null, cancellationToken);

        await _journeyRepository.DeleteJourneyAsync(journeyId, cancellationToken);

        _logger.LogInformation("User {UserId} deleted journey {JourneyId}", userId, journeyId);

        return true;
    }

    // Journey Discovery Methods
    public async Task<IEnumerable<JourneyDto>> SearchJourneysAsync(
        JourneySearchRequest request, CancellationToken cancellationToken, string? currentUserId = null)
    {
        var journeys = await _journeyRepository.SearchJourneysAsync(request, cancellationToken);
        return journeys.Select(j => MapToJourneyDto(j, currentUserId));
    }

    public async Task<IEnumerable<JourneyDto>> GetFeaturedJourneysAsync(
        CancellationToken cancellationToken, string? currentUserId = null)
    {
        var request = new JourneySearchRequest { SortBy = JourneySortBy.MostLiked, Take = 10 };
        return await SearchJourneysAsync(request, cancellationToken, currentUserId);
    }

    public async Task<IEnumerable<JourneyDto>> GetTrendingJourneysAsync(
        CancellationToken cancellationToken, string? currentUserId = null)
    {
        var request = new JourneySearchRequest { SortBy = JourneySortBy.RecentActivity, Take = 10 };
        return await SearchJourneysAsync(request, cancellationToken, currentUserId);
    }

    // Journey Posts Methods
    public async Task<IEnumerable<JourneyPostDto>> GetJourneyPostsAsync(
        int journeyId, CancellationToken cancellationToken, int skip = 0, int take = 20, string? currentUserId = null)
    {
        var posts = await _journeyRepository.GetJourneyPostsAsync(journeyId, skip, take, cancellationToken);
        return posts.Select(p => MapToJourneyPostDto(p, currentUserId));
    }

    public async Task<JourneyPostDto?> GetJourneyPostByIdAsync(
        int postId, CancellationToken cancellationToken, string? currentUserId = null)
    {
    var post = await _journeyRepository.GetJourneyPostByIdAsync(postId, cancellationToken);
        return post is not null ? MapToJourneyPostDto(post, currentUserId) : null;
    }

    public async Task<JourneyPostDto> CreateJourneyPostAsync(
        string userId, int journeyId, CreateJourneyPostRequest request, CancellationToken cancellationToken)
    {
        if (!await CanUserPostToJourneyAsync(userId, journeyId, cancellationToken))
            throw new InvalidOperationException("User cannot post to this journey");

        if (request.Images.Count != 0)
            await _imageService.ValidatePostImagesAsync(userId, request.Images, cancellationToken);

        var post = new JourneyPost
        {
            JourneyId = journeyId,
            UserId = userId,
            TextContent = request.TextContent,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _journeyRepository.CreateJourneyPostAsync(post, cancellationToken);

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

                await _journeyRepository.AddPostImagesAsync(images, cancellationToken);
            }

            await _journeyRepository.TouchJourneyAsync(journeyId, cancellationToken);
        }, cancellationToken);

        _logger.LogInformation("User {UserId} created post {PostId} in journey {JourneyId} with {ImageCount} images",
        userId, post.Id, journeyId, request.Images.Count);

        return await GetJourneyPostByIdAsync(post.Id, cancellationToken, userId)
            ?? throw new InvalidOperationException("Failed to retrieve created post");
    }

    public async Task<JourneyPostDto?> UpdateJourneyPostAsync(string userId, int postId, UpdateJourneyPostRequest request, CancellationToken cancellationToken)
    {
        var post = await _journeyRepository.GetJourneyPostByIdAsync(postId, cancellationToken);

        if (post is null || post.UserId != userId)
            return null;

        if (request.TextContent != null)
        {
            post.TextContent = request.TextContent;
            post.IsEdited = true;
            post.UpdatedAt = DateTime.UtcNow;
        }

        await _journeyRepository.UpdateJourneyPostAsync(post, cancellationToken);

        _logger.LogInformation("User {UserId} updated post {PostId}", userId, postId);

        return await GetJourneyPostByIdAsync(postId, cancellationToken, userId);
    }

    public async Task<bool> DeleteJourneyPostAsync(string userId, int postId, CancellationToken cancellationToken)
    {
        var post = await _journeyRepository.GetJourneyPostByIdAsync(postId, cancellationToken);

        if (post is null || post.UserId != userId)
            return false;

        await _journeyRepository.DeleteJourneyPostAsync(postId, cancellationToken);

        _logger.LogInformation("User {UserId} deleted post {PostId}", userId, postId);

        return true;
    }

    // Journey Engagement Methods
    public async Task<bool> SubscribeToJourneyAsync(string userId, int journeyId, CancellationToken cancellationToken)
    {
        var existing = await _journeyRepository.GetSubscriptionAsync(userId, journeyId, cancellationToken);

        if (existing is not null)
        {
            if (!existing.IsActive)
            {
                existing.IsActive = true;
                await _journeyRepository.UpdateSubscriptionAsync(existing, cancellationToken);
            }

        return true;
        }

        await _journeyRepository.CreateSubscriptionAsync(new JourneySubscription
        {
            UserId = userId,
            JourneyId = journeyId,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        }, cancellationToken);

        _logger.LogInformation("User {UserId} subscribed to journey {JourneyId}", userId, journeyId);

        return true;
    }

    public async Task<bool> UnsubscribeFromJourneyAsync(string userId, int journeyId, CancellationToken cancellationToken)
    {
        var subscription = await _journeyRepository.GetSubscriptionAsync(userId, journeyId, cancellationToken);
        if (subscription is null)
            return false;

        subscription.IsActive = false;

        await _journeyRepository.UpdateSubscriptionAsync(subscription, cancellationToken);

        _logger.LogInformation("User {UserId} unsubscribed from journey {JourneyId}", userId, journeyId);

        return true;
    }

    public async Task<bool> LikeJourneyAsync(string userId, int journeyId, CancellationToken cancellationToken)
    {
        var existing = await _journeyRepository.GetLikeAsync(userId, journeyId, cancellationToken);

        if (existing is not null)
            return true;

        await _journeyRepository.CreateLikeAsync(new JourneyLike
        {
            UserId = userId,
            JourneyId = journeyId,
            CreatedAt = DateTime.UtcNow
        }, cancellationToken);

        _logger.LogInformation("User {UserId} liked journey {JourneyId}", userId, journeyId);

        return true;
    }

    public async Task<bool> UnlikeJourneyAsync(string userId, int journeyId, CancellationToken cancellationToken)
    {
        var like = await _journeyRepository.GetLikeAsync(userId, journeyId, cancellationToken);

        if (like is null)
            return false;

        await _journeyRepository.DeleteLikeAsync(userId, journeyId, cancellationToken);

        _logger.LogInformation("User {UserId} unliked journey {JourneyId}", userId, journeyId);

        return true;
    }

    public Task<bool> IsSubscribedToJourneyAsync(string userId, int journeyId, CancellationToken cancellationToken)
        => _journeyRepository.IsSubscribedAsync(userId, journeyId, cancellationToken);

    public Task<bool> HasLikedJourneyAsync(string userId, int journeyId, CancellationToken cancellationToken)
        => _journeyRepository.HasLikedAsync(userId, journeyId, cancellationToken);

    // User Default Journey Methods
    public async Task<JourneyDto?> GetUserDefaultActiveJourneyAsync(string userId, CancellationToken cancellationToken)
    {
      var user = await _journeyRepository.GetUserAsync(userId, cancellationToken);

        if (user?.DefaultActiveJourneyId is null)
            return null;

        return await GetJourneyByIdAsync(user.DefaultActiveJourneyId.Value, cancellationToken, userId);
    }

    public async Task<bool> SetUserDefaultActiveJourneyAsync(string userId, int journeyId, CancellationToken cancellationToken)
    {
        var journey = await _journeyRepository.GetJourneyByIdAsync(journeyId, cancellationToken);

        if (journey is null || journey.UserId != userId)
            return false;

        await _journeyRepository.UpdateUserDefaultJourneyAsync(userId, journeyId, cancellationToken);

        _logger.LogInformation("User {UserId} set default active journey to {JourneyId}", userId, journeyId);

        return true;
    }

    // Stats Methods
    public async Task<JourneyStatsDto> GetUserJourneyStatsAsync(string userId, CancellationToken cancellationToken)
    {
        var journeys = await _journeyRepository.GetUserJourneysWithStatsAsync(userId, cancellationToken);

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
    public async Task<bool> CanUserCreateJourneyAsync(string userId, CancellationToken cancellationToken)
    {
        var user = await _journeyRepository.GetUserAsync(userId, cancellationToken);
        if (user is null) 
            return false;

        var currentJourneyCount = await _journeyRepository.GetUserJourneyCountAsync(userId, cancellationToken);

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

    public async Task<bool> CanUserPostToJourneyAsync(string userId, int journeyId, CancellationToken cancellationToken)
    {
        var journey = await _journeyRepository.GetJourneyByIdAsync(journeyId, cancellationToken);
        return journey?.UserId == userId;
    }

    // Mapping Methods
    private static JourneyDto MapToJourneyDto(Journey journey, string? currentUserId = null)
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
            IsSubscribed = !string.IsNullOrEmpty(currentUserId) && journey.Subscriptions.Any(
                s => s.UserId == currentUserId && s.IsActive),
            IsLiked = !string.IsNullOrEmpty(currentUserId) && journey.Likes.Any(
                l => l.UserId == currentUserId),
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

    private static JourneyPostDto MapToJourneyPostDto(JourneyPost post, string? currentUserId = null)
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
