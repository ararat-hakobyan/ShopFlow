using FluentValidation;
using ShopFlow.DTOModels;

namespace ShopFlow.Validators;

public sealed class RemoveFromCartRequestValidator : AbstractValidator<RemoveFromCartRequest>
{
    public RemoveFromCartRequestValidator()
    {
        RuleFor(request => request.VariantId)
            .GreaterThan(0).WithMessage("Invalid item.");
    }
}
