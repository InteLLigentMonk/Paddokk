using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;

namespace Paddokk.Core.Features.Journeys.Commands.UpdateJourneyPost;

public sealed class UpdateJourneyPostHandler(IJourneyRepository journeyRepository, IActorResolver actor, IHtmlSanitizationService htmlSanitizer)
    : IRequestHandler<UpdateJourneyPostCommand, Result<JourneyPostDto>>
{
    public async Task<Result<JourneyPostDto>> Handle(UpdateJourneyPostCommand request, CancellationToken cancellationToken)
    {
        var post = await journeyRepository.GetJourneyPostByIdAsync(request.PostId, cancellationToken);

        if (post is null || post.AuthorId != actor.UserId)
            return Result<JourneyPostDto>.Failure(Error.NotFound("Post not found or you don't own it"));

        if (request.TextContent is not null)
        {
            post.TextContent = htmlSanitizer.Sanitize(request.TextContent);
            post.IsEdited = true;
            post.UpdatedAt = DateTime.UtcNow;
        }

        await journeyRepository.UpdateJourneyPostAsync(post, cancellationToken);

        var updated = await journeyRepository.GetJourneyPostByIdAsync(request.PostId, cancellationToken);
        return Result<JourneyPostDto>.Success(JourneyMapping.ToJourneyPostDto(updated!, actor.UserId));
    }
}
