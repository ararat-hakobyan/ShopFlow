using Microsoft.AspNetCore.Identity;
using ShopFlow.Models;
using ShopFlow.Services.Interfaces;

namespace ShopFlow.Services;

public sealed class IdentityPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<User> _hasher = new();

    public string Hash(string password) => _hasher.HashPassword(new User(), password);

    public bool Verify(string hashedPassword, string providedPassword)
    {
        var result = _hasher.VerifyHashedPassword(new User(), hashedPassword, providedPassword);

        return result is PasswordVerificationResult.Success
                      or PasswordVerificationResult.SuccessRehashNeeded;
    }
}
