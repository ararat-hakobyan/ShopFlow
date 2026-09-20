using FluentValidation;
using ShopFlow.DTOModels;

namespace ShopFlow.Validators;

public sealed class AddCategoryRequestValidator : AbstractValidator<AddCategoryRequest>
{
    public AddCategoryRequestValidator()
    {
        RuleFor(request => request.Name)
            .NotEmpty().WithMessage("Category name is required.")
            .MaximumLength(100).WithMessage("Category name cannot be longer than 100 characters.");
    }
}
