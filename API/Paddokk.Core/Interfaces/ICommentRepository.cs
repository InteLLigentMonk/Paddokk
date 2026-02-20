using Paddokk.Core.Models.DTOs.Comment;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Interfaces;

public interface ICommentRepository
{
    Task<(List<PostComment> postComments, int totalCount)> GetPostCommentsAsync(int postId, CancellationToken cancellationToken, int page = 1, int pageSize = 20);

    Task<PostComment?> GetCommentByIdAsync(int commentId, CancellationToken cancellationToken, string? currentUserId = null);

    Task CreateCommentAsync(PostComment comment, CancellationToken cancellationToken);

    Task<bool> UpdateCommentAsync(string userId, int commentId, UpdateCommentRequest request, CancellationToken cancellationToken);

    Task DeleteCommentAsync(PostComment comment, CancellationToken cancellationToken);

    Task<int> GetPostCommentCountAsync(int postId, CancellationToken cancellationToken);

    Task<bool> JourneyPostExists(int postId, CancellationToken cancellationToken);

    Task<bool> UserExist(string userId, CancellationToken cancellationToken);
}
