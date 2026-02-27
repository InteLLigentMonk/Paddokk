using FluentValidation;

namespace Paddokk.Core.Features.Cars.Commands.CreateUserCar;

public sealed class CreateUserCarCommandValidator : AbstractValidator<CreateUserCarCommand>
{
    public CreateUserCarCommandValidator()
    {
        RuleFor(x => x.CarMakeId).GreaterThan(0).WithMessage("Car make is required");
        RuleFor(x => x.CarModelId).GreaterThan(0).WithMessage("Car model is required");
        RuleFor(x => x.Year)
            .GreaterThanOrEqualTo(1900).WithMessage("Year must be 1900 or later")
            .LessThanOrEqualTo(DateTime.UtcNow.Year + 1).WithMessage("Year cannot be in the future");

        RuleFor(x => x.Nickname)
            .MaximumLength(100).When(x => x.Nickname is not null)
            .WithMessage("Nickname cannot exceed 100 characters");

        RuleFor(x => x.Color)
            .MaximumLength(50).When(x => x.Color is not null)
            .WithMessage("Color cannot exceed 50 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).When(x => x.Description is not null)
            .WithMessage("Description cannot exceed 500 characters");
    }
}
