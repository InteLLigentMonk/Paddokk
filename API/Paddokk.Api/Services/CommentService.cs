using API.Data;
using Microsoft.EntityFrameworkCore;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.DTOs;
using Paddokk.Core.Models.Entities;

namespace API.Services;

public class CommentService : ICommentService
{
    private readonly PaddokkDbContext _context;
    private readonly ILogger<CommentService> _logger;

    public CommentService(PaddokkDbContext context, ILogger<CommentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<CommentsPagedResponse> GetPostCommentsAsync(int postId, int page = 1, int pageSize = 20, string? currentUserId = null)
    {
        // Validate post exists
        var postExists = await _context.JourneyPosts.AnyAsync(p => p.Id == postId);
        if (!postExists)
            throw new ArgumentException("Post not found");

        var query = _context.PostComments
            .Include(c => c.User)
            .Include(c => c.JourneyPost)
                .ThenInclude(p => p.User)
            .Where(c => c.JourneyPostId == postId)
            .OrderBy(c => c.CreatedAt); // Chronological order (oldest first)

        var totalCount = await query.CountAsync();

        var comments = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var commentDtos = comments.Select(c => MapToCommentDto(c, currentUserId)).ToList();

        return new CommentsPagedResponse
        {
            Comments = commentDtos,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize,
            HasNext = totalCount > page * pageSize,
            HasPrevious = page > 1
        };
    }

    public async Task<PostCommentDto?> GetCommentByIdAsync(int commentId, string? currentUserId = null)
    {
        var comment = await _context.PostComments
            .Include(c => c.User)
            .Include(c => c.JourneyPost)
                .ThenInclude(p => p.User)
            .FirstOrDefaultAsync(c => c.Id == commentId);

        return comment != null ? MapToCommentDto(comment, currentUserId) : null;
    }

    public async Task<PostCommentDto> CreateCommentAsync(string userId, int postId, CreateCommentRequest request)
    {
        // Validate user can comment on this post
        if (!await CanUserCommentOnPostAsync(userId, postId))
            throw new InvalidOperationException("Cannot comment on this post");

        // Get the post to include in response
        var post = await _context.JourneyPosts
            .Include(p => p.User)
            .FirstOrDefaultAsync(p => p.Id == postId);

        if (post == null)
            throw new ArgumentException("Post not found");

        var comment = new PostComment
        {
            JourneyPostId = postId,
            UserId = userId,
            Content = request.Content.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _context.PostComments.Add(comment);
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} commented on post {PostId}", userId, postId);

        // Return the created comment with full details
        return await GetCommentByIdAsync(comment.Id, userId)
            ?? throw new InvalidOperationException("Failed to retrieve created comment");
    }

    public async Task<PostCommentDto?> UpdateCommentAsync(string userId, int commentId, UpdateCommentRequest request)
    {
        var comment = await _context.PostComments
            .FirstOrDefaultAsync(c => c.Id == commentId && c.UserId == userId);

        if (comment == null)
            return null;

        comment.Content = request.Content.Trim();
        comment.UpdatedAt = DateTime.UtcNow;
        comment.IsEdited = true;

        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} updated comment {CommentId}", userId, commentId);

        return await GetCommentByIdAsync(commentId, userId);
    }

    public async Task<bool> DeleteCommentAsync(string userId, int commentId)
    {
        var comment = await _context.PostComments
            .Include(c => c.JourneyPost)
            .FirstOrDefaultAsync(c => c.Id == commentId);

        if (comment == null)
            return false;

        // User can delete their own comments, or post owner can delete comments on their posts
        var canDelete = comment.UserId == userId || comment.JourneyPost.UserId == userId;

        if (!canDelete)
            return false;

        _context.PostComments.Remove(comment);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Comment {CommentId} deleted by user {UserId}", commentId, userId);

        return true;
    }

    public async Task<int> GetPostCommentCountAsync(int postId)
    {
        return await _context.PostComments.CountAsync(c => c.JourneyPostId == postId);
    }

    public async Task<bool> CanUserCommentOnPostAsync(string userId, int postId)
    {
        // Check if post exists and user exists
        var post = await _context.JourneyPosts
            .Include(p => p.Journey)
            .FirstOrDefaultAsync(p => p.Id == postId);

        if (post == null)
            return false;

        var user = await _context.Users.FindAsync(userId);
        if (user == null || user.IsDeleted)
            return false;

        // For now, any authenticated user can comment
        // Future: Could add restrictions like:
        // - Journey must be public
        // - User must be following the journey
        // - User must not be blocked by post owner

        return true;
    }

    public async Task<bool> UserOwnsCommentAsync(string userId, int commentId)
    {
        return await _context.PostComments
            .AnyAsync(c => c.Id == commentId && c.UserId == userId);
    }

    public async Task<bool> ReportCommentAsync(string userId, int commentId, string reason)
    {
        // Placeholder for future moderation system
        // Could create a CommentReport entity

        _logger.LogWarning("Comment {CommentId} reported by user {UserId} for: {Reason}",
            commentId, userId, reason);

        await Task.CompletedTask;
        return true;
    }

    private PostCommentDto MapToCommentDto(PostComment comment, string? currentUserId = null)
    {
        return new PostCommentDto
        {
            Id = comment.Id,
            JourneyPostId = comment.JourneyPostId,
            UserId = comment.UserId,
            UserDisplayName = comment.User.DisplayName,
            UserAvatarUrl = comment.User.AvatarUrl,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            UpdatedAt = comment.UpdatedAt,
            IsEdited = comment.IsEdited,
            IsOwner = !string.IsNullOrEmpty(currentUserId) && comment.UserId == currentUserId,
            IsPostOwner = comment.UserId == comment.JourneyPost.UserId
        };
    }
}
