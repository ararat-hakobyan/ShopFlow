using FluentValidation;
using ShopFlow.DTOModels;

namespace ShopFlow.Validators;

public sealed class AddCourierRequestValidator : AbstractValidator<AddCourierRequest>
{
    public AddCourierRequestValidator()
    {
        RuleFor(request => request.Username)
            .NotEmpty().WithMessage("Username is required.")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters long.")
            .MaximumLength(50).WithMessage("Username cannot be longer than 50 characters.");

        RuleFor(request => request.Email)
            .NotEmpty().WithMessage("Email address is required.")
            .EmailAddress().WithMessage("Enter a valid email address.");

        RuleFor(request => request.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
            .Matches("[A-Z]").WithMessage("Password must contain at least one capital letter.")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit.");

        RuleFor(request => request.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .MaximumLength(50).WithMessage("First name cannot be longer than 50 characters.");

        RuleFor(request => request.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .MaximumLength(50).WithMessage("Last name cannot be longer than 50 characters.");

        RuleFor(request => request.Phone)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\+?[0-9]{8,15}$").WithMessage("Enter a valid phone number, for example +37499123456.");

        RuleFor(request => request.VehicleType)
            .MaximumLength(30).WithMessage("Vehicle type cannot be longer than 30 characters.");
    }
}
