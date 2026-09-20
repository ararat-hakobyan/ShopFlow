using FluentValidation;
using ShopFlow.DTOModels;

namespace ShopFlow.Validators;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(request => request.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(50).WithMessage("Username cannot be longer than 50 characters.");

        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Enter a valid email address.")
            .MaximumLength(255).WithMessage("Email address cannot be longer than 255 characters.");

        RuleFor(request => request.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one capital letter.")
            .Matches("[a-z]").WithMessage("Password must contain at least one small letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");

        RuleFor(request => request.ConfirmPassword)
            .Equal(request => request.Password).WithMessage("The two passwords do not match.");
    }
}
