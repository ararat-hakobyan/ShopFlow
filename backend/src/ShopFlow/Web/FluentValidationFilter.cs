using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using ShopFlow.Common;
using ShopFlow.Extensions;

namespace ShopFlow.Web;

public sealed class FluentValidationFilter : IAsyncActionFilter
{
    private readonly IServiceProvider _serviceProvider;

    public FluentValidationFilter(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        foreach (var argument in context.ActionArguments.Values)
        {
            if (argument is null)
            {
                continue;
            }

            var validator = FindValidatorFor(argument);

            if (validator is null)
            {
                continue;
            }

            // ValidationContext<object> is how FluentValidation validates an instance
            // whose type is only known at run time.
            var validationContext = new ValidationContext<object>(argument);

            var result = await validator.ValidateAsync(validationContext, context.HttpContext.RequestAborted);

            foreach (var failure in result.Errors)
            {
                context.ModelState.AddModelError(failure.PropertyName, failure.ErrorMessage);
            }
        }

        if (!context.ModelState.IsValid)
        {
            context.Result = new BadRequestObjectResult(
                ApiResponse.Fail(context.ModelState.ToSingleMessage(), context.ModelState.ToErrorDictionary()));

            return;
        }

        await next();
    }

    // Validators are registered as IValidator<TRequest> and the argument type is only
    // known at run time, so the closed type has to be built to ask the container for it.
    private IValidator? FindValidatorFor(object argument)
    {
        var validatorType = typeof(IValidator<>).MakeGenericType(argument.GetType());

        return _serviceProvider.GetService(validatorType) as IValidator;
    }
}
