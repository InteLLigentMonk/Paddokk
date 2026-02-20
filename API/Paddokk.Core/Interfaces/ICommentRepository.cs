using Paddokk.Core.Models.DTOs.Comment;

namespace Paddokk.Core.Interfaces;

public interface ICommentRepository
{
    Task<CommentsPagedResponse> GetPostCommentsAsync(int postId, CancellationToken cancellationToken, int page = 1, int pageSize = 20);

    Task<PostCommentDto?> GetCommentByIdAsync(int commentId, CancellationToken cancellationToken, string? currentUserId = null);

    Task<PostCommentDto> CreateCommentAsync(string userId, int postId, CreateCommentRequest request, CancellationToken cancellationToken);

    Task<PostCommentDto?> UpdateCommentAsync(string userId, int commentId, UpdateCommentRequest request, CancellationToken cancellationToken);

    Task<bool> DeleteCommentAsync(string userId, int commentId, CancellationToken cancellationToken);

    Task<int> GetPostCommentCountAsync(int postId, CancellationToken cancellationToken);

    Task<bool> JourneyPostExists(int postId, CancellationToken cancellationToken);

    Task<bool> UserExist(string userId, CancellationToken cancellationToken);
}
