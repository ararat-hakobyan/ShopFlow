using ShopFlow.Services.Interfaces;

namespace ShopFlow.Services;

public sealed class SystemDateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}
