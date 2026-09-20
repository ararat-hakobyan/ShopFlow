using ShopFlow.Services.Interfaces;

namespace ShopFlow.Tests;

// A clock that never moves, so a test can say exactly what "now" is.
// This is the reason IDateTimeProvider exists instead of DateTime.UtcNow.
public sealed class FixedDateTimeProvider : IDateTimeProvider
{
    public FixedDateTimeProvider(DateTime utcNow)
    {
        UtcNow = utcNow;
    }

    public DateTime UtcNow { get; }
}
