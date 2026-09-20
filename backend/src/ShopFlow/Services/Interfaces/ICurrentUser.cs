using ShopFlow.Enums;

namespace ShopFlow.Services.Interfaces;

public interface ICurrentUser
{
    int UserId { get; }

    string Username { get; }

    UserRole Role { get; }
}
