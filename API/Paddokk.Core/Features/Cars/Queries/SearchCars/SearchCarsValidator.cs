using FluentValidation;

namespace Paddokk.Core.Features.Cars.Queries.SearchCars;

public class SearchCarsValidator : AbstractValidator<SearchCarsQuery>
{
    public SearchCarsValidator()
    {
        RuleFor(x => x.Terms)
            .Must(terms => terms.Count <= 10)
            .WithMessage("Maximum 10 search terms allowed");

        RuleForEach(x => x.Terms)
            .MaximumLength(50);

        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1);

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, 50);
    }
}
