using ShopFlow.Services;
using Xunit;

namespace ShopFlow.Tests;

public class IdentityPasswordHasherTests
{
    private readonly IdentityPasswordHasher _hasher = new();

    [Fact]
    public void Verify_ReturnsTrue_ForTheSamePassword()
    {
        var hash = _hasher.Hash("Admin123!");

        Assert.True(_hasher.Verify(hash, "Admin123!"));
    }

    [Fact]
    public void Verify_ReturnsFalse_ForADifferentPassword()
    {
        var hash = _hasher.Hash("Admin123!");

        Assert.False(_hasher.Verify(hash, "admin123!"));
    }

    [Fact]
    public void Hash_ProducesADifferentValueEachTime()
    {
        // The hash carries a random salt, so two hashes of one password never match.
        Assert.NotEqual(_hasher.Hash("Admin123!"), _hasher.Hash("Admin123!"));
    }
}
