namespace ShopFlow.DTOModels;

public sealed class LoginResponse
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiresAtUtc { get; set; }

    public AuthenticatedUser User { get; set; } = new();
}
