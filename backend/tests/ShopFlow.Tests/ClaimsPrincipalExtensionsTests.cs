using System.Security.Claims;
using ShopFlow.Extensions;
using Xunit;

namespace ShopFlow.Tests;

public class ClaimsPrincipalExtensionsTests
{
    private static ClaimsPrincipal PrincipalWith(params Claim[] claims)
        => new(new ClaimsIdentity(claims, "Test"));

    [Fact]
    public void GetCourierId_ReturnsNull_WhenTheClaimIsMissing()
    {
        var principal = PrincipalWith(new Claim(ClaimTypes.Name, "customer"));

        Assert.Null(principal.GetCourierId());
    }

    [Fact]
    public void GetCourierId_ReturnsTheNumber_WhenTheClaimIsThere()
    {
        var principal = PrincipalWith(new Claim(ClaimsPrincipalExtensions.CourierIdClaimType, "3"));

        Assert.Equal(3, principal.GetCourierId());
    }
}
