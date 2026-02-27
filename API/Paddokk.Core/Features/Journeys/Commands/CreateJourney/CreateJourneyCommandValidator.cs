using FluentValidation;
using Paddokk.Core.Models.Entities;

namespace Paddokk.Core.Features.Journeys.Commands.CreateJourney;

public sealed class CreateJourneyCommandValidator : AbstractValidator<CreateJourneyCommand>
{
    public CreateJourneyCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000).When(x => x.Description is not null)
            .WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Invalid journey category");

        RuleFor(x => x.UserCarId)
            .GreaterThan(0).WithMessage("A car is required for the journey");
    }
}
