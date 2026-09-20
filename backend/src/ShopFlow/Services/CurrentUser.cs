using System.Security.Claims;
using ShopFlow.Enums;
using ShopFlow.Extensions;
using ShopFlow.Services.Interfaces;

namespace ShopFlow.Services;

public sealed class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    // Only read on endpoints that require authentication, so a missing id means the
    // application is wired up wrongly rather than that a visitor did something unusual.
    public int UserId => Principal?.GetUserId()
        ?? throw new InvalidOperationException(
            "The request carries no user id. This property may only be used on an endpoint that requires authentication.");

    public string Username => Principal?.FindFirstValue(ClaimTypes.Name) ?? string.Empty;

    public UserRole Role => Principal?.GetRole() ?? UserRole.Customer;
}
