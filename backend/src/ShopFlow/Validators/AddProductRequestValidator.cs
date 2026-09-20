using FluentValidation;
using ShopFlow.DTOModels;

namespace ShopFlow.Validators;

public sealed class AddProductRequestValidator : AbstractValidator<AddProductRequest>
{
    public AddProductRequestValidator()
    {
        RuleFor(request => request.ProductName)
            .NotEmpty().WithMessage("Product name is required.")
            .MaximumLength(150).WithMessage("Product name cannot be longer than 150 characters.");

        RuleFor(request => request.CategoryId)
            .GreaterThan(0).WithMessage("Choose a category.");
    }
}
