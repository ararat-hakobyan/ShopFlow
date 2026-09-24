using FluentValidation;
using ShopFlow.DTOModels;

namespace ShopFlow.Validators;

public sealed class PageRequestValidator : AbstractValidator<PageRequest>
{
    public const int MaxPageSize = 50;

    public PageRequestValidator()
    {
        RuleFor(request => request.Page)
            .GreaterThanOrEqualTo(1).WithMessage("Page must be 1 or greater.");

        RuleFor(request => request.PageSize)
            .InclusiveBetween(1, MaxPageSize).WithMessage($"Page size must be between 1 and {MaxPageSize}.");
    }
}
