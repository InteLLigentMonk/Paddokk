using FluentValidation;

namespace Paddokk.Core.Features.Journeys.Commands.CreateJourneyPost;

public sealed class CreateJourneyPostCommandValidator : AbstractValidator<CreateJourneyPostCommand>
{
    public CreateJourneyPostCommandValidator()
    {
        RuleFor(x => x.TextContent)
            .MaximumLength(5000).When(x => x.TextContent is not null)
            .WithMessage("Post content cannot exceed 5000 characters");

        RuleFor(x => x).Must(x => x.TextContent is not null || x.Images.Count > 0)
            .WithMessage("A post must have text content or at least one image");
    }
}
