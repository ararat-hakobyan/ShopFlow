using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopFlow.Common;
using ShopFlow.Enums;
using ShopFlow.Extensions;
using ShopFlow.Services.Interfaces;

namespace ShopFlow.Controllers;

[Authorize(Roles = nameof(UserRole.Courier))]
public sealed class CourierController : ApiControllerBase
{
    private readonly ICourierService _courierService;

    public CourierController(ICourierService courierService)
    {
        _courierService = courierService;
    }

    [HttpGet("console")]
    public async Task<IActionResult> Console()
    {
        if (User.GetCourierId() is not int courierId)
        {
            return NoCourierProfile();
        }

        var console = await _courierService.GetConsoleAsync(courierId, RequestAborted);

        return Ok(console);
    }

    [HttpPost("orders/{orderId:int}/accept")]
    public async Task<IActionResult> AcceptOrder(int orderId)
    {
        if (User.GetCourierId() is not int courierId)
        {
            return NoCourierProfile();
        }

        return FromResult(await _courierService.AcceptOrderAsync(orderId, courierId, RequestAborted));
    }

    [HttpPost("orders/{orderId:int}/complete")]
    public async Task<IActionResult> CompleteOrder(int orderId)
    {
        if (User.GetCourierId() is not int courierId)
        {
            return NoCourierProfile();
        }

        return FromResult(await _courierService.CompleteOrderAsync(orderId, courierId, RequestAborted));
    }

    private IActionResult NoCourierProfile()
        => StatusCode(
            StatusCodes.Status403Forbidden,
            ApiResponse.Fail("Your account does not have permission to open this page."));
}
