using ShopFlow.DTOModels;
using Xunit;

namespace ShopFlow.Tests;

public class BasketDtoTests
{
    [Fact]
    public void Total_AddsUpEveryLine()
    {
        var basket = new BasketDto
        {
            Items =
            {
                new BasketItemDto { UnitPrice = 1500m, Quantity = 2 },
                new BasketItemDto { UnitPrice = 500m, Quantity = 3 }
            }
        };

        Assert.Equal(4500m, basket.Total);
        Assert.Equal(2, basket.ItemCount);
        Assert.False(basket.IsEmpty);
    }

    [Fact]
    public void ANewBasketIsEmpty()
    {
        var basket = new BasketDto();

        Assert.True(basket.IsEmpty);
        Assert.Equal(0m, basket.Total);
    }
}
