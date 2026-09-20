using FluentValidation;
using ShopFlow.DTOModels;

namespace ShopFlow.Validators;

public sealed class UpdateVariantPriceRequestValidator : AbstractValidator<UpdateVariantPriceRequest>
{
    public UpdateVariantPriceRequestValidator()
    {
        RuleFor(request => request.VariantId)
            .GreaterThan(0).WithMessage("Invalid variant.");

        RuleFor(request => request.NewPrice)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
    }
}
