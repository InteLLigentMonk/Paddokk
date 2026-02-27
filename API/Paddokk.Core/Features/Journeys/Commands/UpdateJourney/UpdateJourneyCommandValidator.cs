using FluentValidation;

namespace Paddokk.Core.Features.Journeys.Commands.UpdateJourney;

public sealed class UpdateJourneyCommandValidator : AbstractValidator<UpdateJourneyCommand>
{
    public UpdateJourneyCommandValidator()
    {
        RuleFor(x => x.Title)
            .MinimumLength(3).When(x => x.Title is not null).WithMessage("Title must be at least 3 characters")
            .MaximumLength(200).When(x => x.Title is not null).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).When(x => x.Description is not null)
            .WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.Category)
            .IsInEnum().When(x => x.Category.HasValue)
            .WithMessage("Invalid journey category");

        RuleFor(x => x.Status)
            .IsInEnum().When(x => x.Status.HasValue)
            .WithMessage("Invalid journey status");

        RuleFor(x => x.CompletedAt)
            .LessThanOrEqualTo(DateTime.UtcNow).When(x => x.CompletedAt.HasValue)
            .WithMessage("Completion date cannot be in the future");
    }
}
