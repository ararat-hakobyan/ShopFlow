using System.Security.Claims;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.Extensions.Options;
using ShopFlow.DTOModels;
using ShopFlow.Enums;
using ShopFlow.Extensions;
using ShopFlow.Services;
using ShopFlow.Web;
using Xunit;

namespace ShopFlow.Tests;

public class JwtTokenServiceTests
{
    private static readonly DateTime Now = new(2026, 1, 1, 12, 0, 0, DateTimeKind.Utc);

    private static JwtTokenService CreateService() => new(
        Options.Create(new JwtOptions
        {
            Issuer = "ShopFlow",
            Audience = "ShopFlow.Frontend",
            SigningKey = "a-signing-key-that-is-long-enough-for-hmac-sha256",
            ExpiresInHours = 8
        }),
        new FixedDateTimeProvider(Now));

    [Fact]
    public void Create_SetsTheExpiryEightHoursAfterNow()
    {
        var token = CreateService().Create(new AuthenticatedUser
        {
            UserId = 1,
            Username = "admin",
            Email = "admin@shopflow.local",
            Role = UserRole.Admin
        });

        Assert.Equal(Now.AddHours(8), token.ExpiresAtUtc);
    }

    [Fact]
    public void Create_PutsTheCourierIdInTheTokenForACourier()
    {
        var token = CreateService().Create(new AuthenticatedUser
        {
            UserId = 7,
            Username = "courier",
            Email = "courier@shopflow.local",
            Role = UserRole.Courier,
            CourierId = 3
        });

        var claims = new JsonWebTokenHandler().ReadJsonWebToken(token.Token).Claims.ToList();

        Assert.Equal("3", claims.Single(c => c.Type == ClaimsPrincipalExtensions.CourierIdClaimType).Value);
    }

    [Fact]
    public void Create_LeavesOutTheCourierIdForACustomer()
    {
        var token = CreateService().Create(new AuthenticatedUser
        {
            UserId = 9,
            Username = "customer",
            Email = "customer@shopflow.local",
            Role = UserRole.Customer
        });

        var claims = new JsonWebTokenHandler().ReadJsonWebToken(token.Token).Claims;

        Assert.DoesNotContain(claims, c => c.Type == ClaimsPrincipalExtensions.CourierIdClaimType);
    }
}
