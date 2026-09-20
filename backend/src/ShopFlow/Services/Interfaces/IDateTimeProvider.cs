namespace ShopFlow.Services.Interfaces;

public interface IDateTimeProvider
{
    DateTime UtcNow { get; }
}
