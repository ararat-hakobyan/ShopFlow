using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopFlow.Common;
using ShopFlow.DTOModels;
using ShopFlow.Enums;
using ShopFlow.Services.Interfaces;

namespace ShopFlow.Controllers;

[Authorize(Roles = nameof(UserRole.Customer))]
public sealed class ShopController : ApiControllerBase
{
    private readonly IShopService _shopService;
    private readonly ICurrentUser _currentUser;

    public ShopController(IShopService shopService, ICurrentUser currentUser)
    {
        _shopService = shopService;
        _currentUser = currentUser;
    }

    [HttpGet("storefront")]
    public async Task<IActionResult> Storefront(int? categoryId = null, int? productId = null)
    {
        var result = await _shopService.GetStorefrontAsync(
            _currentUser.UserId,
            categoryId,
            productId,
            RequestAborted);

        return FromResult(result);
    }

    [HttpPost("cart/items")]
    public async Task<IActionResult> AddToCart(AddToCartRequest request)
    {
        // The user id comes from the token, never from the request body.
        var result = await _shopService.AddToCartAsync(_currentUser.UserId, request, RequestAborted);

        return FromResult(result);
    }

    [HttpPost("cart/items/remove")]
    public async Task<IActionResult> RemoveFromCart(RemoveFromCartRequest request)
    {
        var result = await _shopService.RemoveFromCartAsync(_currentUser.UserId, request, RequestAborted);

        return FromResult(result);
    }

    [HttpPost("orders")]
    public async Task<IActionResult> PlaceOrder()
    {
        var result = await _shopService.PlaceOrderAsync(_currentUser.UserId, RequestAborted);

        return FromResult((Result)result);
    }

    [HttpPost("orders/{id:int}/cancel")]
    public async Task<IActionResult> CancelOrder(int id)
    {
        var result = await _shopService.CancelOrderAsync(_currentUser.UserId, id, RequestAborted);

        return FromResult(result);
    }

    [HttpGet("orders/{id:int}")]
    public async Task<IActionResult> OrderDetails(int id)
    {
        var result = await _shopService.GetOrderForUserAsync(_currentUser.UserId, id, RequestAborted);

        return FromResult(result);
    }
}
