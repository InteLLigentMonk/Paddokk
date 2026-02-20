using Paddokk.Core.Interfaces;
using Paddokk.Core.Models.Entities;
using Paddokk.Core.Models.DTOs.Comment;
using Microsoft.Extensions.Logging;

namespace Paddokk.Core.Services;

public class CommentService : ICommentService
{
    private readonly ILogger<CommentService> _logger;
    private readonly ICommentRepository _commentRepository;

    public CommentService(ICommentRepository commentRepository, ILogger<CommentService> logger)
    {
        _commentRepository = commentRepository;
        _logger = logger;
    }

    public async Task<CommentsPagedResponse> GetPostCommentsAsync(int postId, CancellationToken cancellationToken, int page = 1, int pageSize = 20, string? currentUserId = null)
    {
        var (postComments, totalCount) = await _commentRepository.GetPostCommentsAsync(postId, cancellationToken, page, pageSize);

        var commentDtos = postComments.Select(c => MapToCommentDto(c, currentUserId)).ToList();

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

    public async Task<PostCommentDto?> GetCommentByIdAsync(int commentId, CancellationToken cancellationToken, string? currentUserId = null)
    {
        var comment = await _commentRepository.GetCommentByIdAsync(commentId, cancellationToken, currentUserId);

        return comment != null ? MapToCommentDto(comment, currentUserId) : null;
    }

    public async Task<PostCommentDto> CreateCommentAsync(string userId, int postId, CreateCommentRequest request, CancellationToken cancellationToken)
    {
        // Validate user can comment on this post
        if (!await CanUserCommentOnPostAsync(userId, postId, cancellationToken))
            throw new InvalidOperationException("Cannot comment on this post");

        var comment = new PostComment
        {
            JourneyPostId = postId,
            UserId = userId,
            Content = request.Content.Trim(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        
        await _commentRepository.CreateCommentAsync(comment, cancellationToken);

        _logger.LogInformation("User {UserId} commented on post {PostId}", userId, postId);

        // Return the created comment with full details
        return await GetCommentByIdAsync(comment.Id, cancellationToken, userId)
            ?? throw new InvalidOperationException("Failed to retrieve created comment");
    }

    public async Task<PostCommentDto?> UpdateCommentAsync(string userId, int commentId, UpdateCommentRequest request, CancellationToken cancellationToken)
    {
        var update = await _commentRepository.UpdateCommentAsync(userId, commentId, request, cancellationToken);
        if (!update)
            return null;

        _logger.LogInformation("User {UserId} updated comment {CommentId}", userId, commentId);

        return await GetCommentByIdAsync(commentId, cancellationToken, userId);
    }

    public async Task<bool> DeleteCommentAsync(string userId, int commentId, CancellationToken cancellationToken)
    {

        var comment = await _commentRepository.GetCommentByIdAsync(commentId, cancellationToken);

        if (comment == null)
            return false;

        // User can delete their own comments, or post owner can delete comments on their posts
        var canDelete = comment.UserId == userId || comment.JourneyPost.UserId == userId;

        if (!canDelete)
            return false;

        await _commentRepository.DeleteCommentAsync(comment, cancellationToken);

        _logger.LogInformation("Comment {CommentId} deleted by user {UserId}", commentId, userId);

        return true;
    }

    public async Task<int> GetPostCommentCountAsync(int postId, CancellationToken cancellationToken)
        => await _commentRepository.GetPostCommentCountAsync(postId, cancellationToken);

    public async Task<bool> CanUserCommentOnPostAsync(string userId, int postId, CancellationToken cancellationToken)
    {
        // Check if post exists and user exists
        if (!await _commentRepository.JourneyPostExists(postId, cancellationToken))
            return false;
        if (!await _commentRepository.UserExist(userId, cancellationToken))
            return false;

        // For now, any authenticated user can comment
        // Future: Could add restrictions like:
        // - Journey must be public
        // - User must be following the journey
        // - User must not be blocked by post owner

        return true;
    }

    public async Task<bool> ReportCommentAsync(string userId, int commentId, string reason, CancellationToken cancellationToken)
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
