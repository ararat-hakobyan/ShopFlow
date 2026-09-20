using ShopFlow.Enums;

namespace ShopFlow.DTOModels;

public sealed class AuthenticatedUser
{
    public int UserId { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public UserRole Role { get; set; }

    public int? CourierId { get; set; }
}
