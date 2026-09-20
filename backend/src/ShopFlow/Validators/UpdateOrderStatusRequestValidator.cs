using FluentValidation;
using ShopFlow.DTOModels;
using ShopFlow.Enums;

namespace ShopFlow.Validators;

public sealed class UpdateOrderStatusRequestValidator : AbstractValidator<UpdateOrderStatusRequest>
{
    public UpdateOrderStatusRequestValidator()
    {
        RuleFor(request => request.OrderId)
            .GreaterThan(0).WithMessage("Invalid order.");

        RuleFor(request => request.Status)
            .IsInEnum().WithMessage("Choose a valid status.");

        RuleFor(request => request.CourierId)
            .NotNull().WithMessage("Choose a courier for this delivery.")
            .GreaterThan(0).WithMessage("Choose a courier for this delivery.")
            .When(request => request.Status == OrderStatus.OutForDelivery);
    }
}
