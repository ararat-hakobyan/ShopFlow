using System.Security.Claims;
using ShopFlow.Enums;

namespace ShopFlow.Extensions;

public static class ClaimsPrincipalExtensions
{
    public const string CourierIdClaimType = "shopflow:courier-id";

    // Nullable on purpose: a claim can be missing, and returning 0 would look like an id.
    public static int? GetUserId(this ClaimsPrincipal principal)
        => ReadInt(principal.FindFirstValue(ClaimTypes.NameIdentifier));

    public static int? GetCourierId(this ClaimsPrincipal principal)
        => ReadInt(principal.FindFirstValue(CourierIdClaimType));

    public static UserRole? GetRole(this ClaimsPrincipal principal)
    {
        var value = principal.FindFirstValue(ClaimTypes.Role);

        return Enum.TryParse<UserRole>(value, ignoreCase: true, out var role) ? role : null;
    }

    private static int? ReadInt(string? value)
        => int.TryParse(value, out var parsed) ? parsed : null;
}
