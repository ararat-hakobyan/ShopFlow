using FluentValidation;
using ShopFlow.DTOModels;

namespace ShopFlow.Validators;

public sealed class AddToCartRequestValidator : AbstractValidator<AddToCartRequest>
{
    public AddToCartRequestValidator()
    {
        RuleFor(request => request.VariantId)
            .GreaterThan(0).WithMessage("Choose a size and colour first.");

        RuleFor(request => request.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be at least 1.")
            .LessThanOrEqualTo(100).WithMessage("You cannot order more than 100 of the same item at once.");
    }
}
