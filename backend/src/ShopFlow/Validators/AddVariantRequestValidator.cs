using FluentValidation;
using ShopFlow.DTOModels;

namespace ShopFlow.Validators;

public sealed class AddVariantRequestValidator : AbstractValidator<AddVariantRequest>
{
    public AddVariantRequestValidator()
    {
        RuleFor(request => request.ProductId)
            .GreaterThan(0).WithMessage("Choose a product.");

        RuleFor(request => request.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");

        RuleFor(request => request.StockQuantity)
            .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");

        RuleFor(request => request.Color)
            .MaximumLength(50).WithMessage("Colour cannot be longer than 50 characters.");

        RuleFor(request => request.Size)
            .MaximumLength(20).WithMessage("Size cannot be longer than 20 characters.");
    }
}
