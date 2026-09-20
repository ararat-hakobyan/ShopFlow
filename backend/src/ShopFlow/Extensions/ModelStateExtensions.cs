using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ShopFlow.Extensions;

public static class ModelStateExtensions
{
    public static string ToSingleMessage(this ModelStateDictionary modelState)
        => string.Join(" ", modelState.Values
            .SelectMany(entry => entry.Errors)
            .Select(error => error.ErrorMessage)
            .Where(message => !string.IsNullOrWhiteSpace(message))
            .Distinct());

    public static IDictionary<string, string[]> ToErrorDictionary(this ModelStateDictionary modelState)
        => modelState
            .Where(entry => entry.Value is not null && entry.Value.Errors.Count > 0)
            .ToDictionary(
                entry => entry.Key,
                entry => entry.Value!.Errors
                    .Select(error => error.ErrorMessage)
                    .Where(message => !string.IsNullOrWhiteSpace(message))
                    .ToArray());
}
