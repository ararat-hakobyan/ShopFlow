using ShopFlow.Common;
using Xunit;

namespace ShopFlow.Tests;

public class ResultTests
{
    [Fact]
    public void NotFound_IsAFailureThatKeepsItsReason()
    {
        var result = Result.NotFound("Order 42 was not found.");

        Assert.False(result.IsSuccess);
        Assert.True(result.IsFailure);
        Assert.Equal(ErrorType.NotFound, result.ErrorType);
        Assert.Equal("Order 42 was not found.", result.Message);
    }

    [Fact]
    public void Success_CarriesItsValue()
    {
        var result = Result<int>.Success(42);

        Assert.True(result.IsSuccess);
        Assert.Equal(ErrorType.None, result.ErrorType);
        Assert.Equal(42, result.Value);
    }
}
