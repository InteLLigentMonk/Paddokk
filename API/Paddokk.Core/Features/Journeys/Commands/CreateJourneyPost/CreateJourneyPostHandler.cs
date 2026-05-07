using MediatR;
using Paddokk.Core.Interfaces;
using Paddokk.Core.Models;
using Paddokk.Core.Models.DTOs.Journey;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Journeys.Commands.CreateJourneyPost;

public sealed class CreateJourneyPostHandler(
    IJourneyRepository journeyRepository,
    IImageService imageService,
    IActorResolver actor,
    IHtmlSanitizationService htmlSanitizer)
    : IRequestHandler<CreateJourneyPostCommand, Result<JourneyPostDto>>
{
    public async Task<Result<JourneyPostDto>> Handle(CreateJourneyPostCommand request, CancellationToken cancellationToken)
    {
        var journey = await journeyRepository.GetJourneyByIdAsync(request.JourneyId, cancellationToken);

        if (journey is null || journey.PrincipalId != actor.UserId)
            return Result<JourneyPostDto>.Failure(Error.NotFound("Journey not found or you don't own it"));

        if (journey.Status == JourneyStatus.Completed)
            return Result<JourneyPostDto>.Failure(Error.Validation("Cannot add posts to a completed journey"));

        if (request.Images.Count > 0)
            await imageService.ValidatePostImagesAsync(actor.UserId, request.Images, cancellationToken);

        var post = new JourneyPost
        {
            JourneyId = request.JourneyId,
            AuthorId = actor.UserId,
            TextContent = htmlSanitizer.Sanitize(request.TextContent),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        await journeyRepository.CreateJourneyPostAsync(post, cancellationToken);

        if (request.Images.Count > 0)
        {
            var images = request.Images.Select(img => new JourneyPostImage
            {
                JourneyPostId = post.Id,
                ImageUrl = img.ImageUrl,
                Caption = img.Caption,
                SortOrder = img.SortOrder,
                CreatedAt = DateTime.UtcNow
            }).ToList();

            await journeyRepository.AddPostImagesAsync(images, cancellationToken);
        }

        await journeyRepository.TouchJourneyAsync(request.JourneyId, cancellationToken);

        var created = await journeyRepository.GetJourneyPostByIdAsync(post.Id, cancellationToken);
        return created is null
            ? Result<JourneyPostDto>.Failure(Error.Internal("Failed to retrieve created post"))
            : Result<JourneyPostDto>.Success(JourneyMapping.ToJourneyPostDto(created, actor.UserId));
    }
}
