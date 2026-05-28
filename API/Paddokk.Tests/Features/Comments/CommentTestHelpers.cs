using Paddokk.Core.Models.Entities;
using Paddokk.Tests.Features.Journeys;

namespace Paddokk.Tests.Features.Comments;

internal static class CommentTestHelpers
{
    internal static JourneyPost BuildPost(int postId = 100, string postAuthorId = "owner-1") => new()
    {
        Id = postId,
        JourneyId = 1,
        AuthorId = postAuthorId,
        Author = JourneyTestHelpers.BuildUser(postAuthorId)
    };

    internal static PostComment BuildComment(
        int id,
        int postId,
        string authorId,
        string postAuthorId = "owner-1",
        int? parentCommentId = null) => new()
    {
        Id = id,
        JourneyPostId = postId,
        AuthorId = authorId,
        Content = "hi",
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        Author = JourneyTestHelpers.BuildUser(authorId),
        JourneyPost = BuildPost(postId, postAuthorId),
        ParentCommentId = parentCommentId,
        Replies = []
    };
}
