using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using ShopFlow.DTOModels;
using ShopFlow.Enums;
using ShopFlow.Extensions;
using ShopFlow.Web;
using ShopFlow.Services.Interfaces;

namespace ShopFlow.Services;

public sealed class JwtTokenService : ITokenService
{
    private readonly JwtOptions _options;
    private readonly IDateTimeProvider _dateTimeProvider;

    public JwtTokenService(IOptions<JwtOptions> options, IDateTimeProvider dateTimeProvider)
    {
        _options = options.Value;
        _dateTimeProvider = dateTimeProvider;
    }

    public AccessToken Create(AuthenticatedUser user)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new(ClaimTypes.Name, user.Username),
            new(ClaimTypes.Email, user.Email),
            new(ClaimTypes.Role, user.Role.ToString())
        };

        if (user.Role == UserRole.Courier && user.CourierId.HasValue)
        {
            claims.Add(new Claim(ClaimsPrincipalExtensions.CourierIdClaimType, user.CourierId.Value.ToString()));
        }

        var issuedAt = _dateTimeProvider.UtcNow;
        var expiresAt = issuedAt.AddHours(_options.ExpiresInHours);

        var descriptor = new SecurityTokenDescriptor
        {
            Issuer = _options.Issuer,
            Audience = _options.Audience,
            Subject = new ClaimsIdentity(claims),
            IssuedAt = issuedAt,
            NotBefore = issuedAt,
            Expires = expiresAt,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.SigningKey)),
                SecurityAlgorithms.HmacSha256)
        };

        return new AccessToken
        {
            Token = new JsonWebTokenHandler().CreateToken(descriptor),
            ExpiresAtUtc = expiresAt
        };
    }
}
