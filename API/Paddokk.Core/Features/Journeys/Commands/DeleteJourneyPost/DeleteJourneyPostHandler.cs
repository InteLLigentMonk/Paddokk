using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;

namespace Paddokk.Core.Features.Journeys.Commands.DeleteJourneyPost;

public sealed class DeleteJourneyPostHandler(IJourneyRepository journeyRepository, IActorResolver actor)
    : IRequestHandler<DeleteJourneyPostCommand, Result>
{
    public async Task<Result> Handle(DeleteJourneyPostCommand request, CancellationToken cancellationToken)
    {
        var post = await journeyRepository.GetJourneyPostByIdAsync(request.PostId, cancellationToken);

        if (post is null || post.UserId != actor.UserId)
            return Result.Failure(Error.NotFound("Post not found or you don't own it"));

        await journeyRepository.DeleteJourneyPostAsync(request.PostId, cancellationToken);
        return Result.Success();
    }
}
