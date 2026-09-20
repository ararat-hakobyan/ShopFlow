namespace ShopFlow.Web;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "ShopFlow";

    public string Audience { get; set; } = "ShopFlow.Frontend";

    public string SigningKey { get; set; } = string.Empty;

    public int ExpiresInHours { get; set; } = 8;
}
