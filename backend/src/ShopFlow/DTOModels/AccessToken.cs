namespace ShopFlow.DTOModels;

public sealed class AccessToken
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }
}
