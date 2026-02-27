using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Comment;

namespace Paddokk.Core.Features.Comments.Queries.GetCommentById;

public record GetCommentByIdQuery(int CommentId) : IQuery<Result<PostCommentDto>>;
