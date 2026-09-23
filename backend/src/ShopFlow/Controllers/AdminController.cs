using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShopFlow.DTOModels;
using ShopFlow.Enums;
using ShopFlow.Services.Interfaces;

namespace ShopFlow.Controllers;

[Authorize(Roles = nameof(UserRole.Admin))]
public sealed class AdminController : ApiControllerBase
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard(string? search = null)
    {
        var dashboard = await _adminService.GetDashboardAsync(search, RequestAborted);

        return Ok(dashboard);
    }

    [HttpPost("categories")]
    public async Task<IActionResult> AddCategory(AddCategoryRequest request)
        => FromResult(await _adminService.AddCategoryAsync(request, RequestAborted));

    [HttpPost("products")]
    public async Task<IActionResult> AddProduct(AddProductRequest request)
        => FromResult(await _adminService.AddProductAsync(request, RequestAborted));

    [HttpPost("variants")]
    public async Task<IActionResult> AddVariant(AddVariantRequest request)
        => FromResult(await _adminService.AddVariantAsync(request, RequestAborted));

    [HttpPost("couriers")]
    public async Task<IActionResult> AddCourier(AddCourierRequest request)
        => FromResult(await _adminService.AddCourierAsync(request, RequestAborted));

    [HttpPut("orders/status")]
    public async Task<IActionResult> UpdateOrderStatus(UpdateOrderStatusRequest request)
        => FromResult(await _adminService.UpdateOrderStatusAsync(request, RequestAborted));

    [HttpPut("variants/price")]
    public async Task<IActionResult> UpdateVariantPrice(UpdateVariantPriceRequest request)
        => FromResult(await _adminService.UpdateVariantPriceAsync(request, RequestAborted));

    [HttpDelete("products/{id:int}")]
    public async Task<IActionResult> DeleteProduct(int id)
        => FromResult(await _adminService.DeleteProductAsync(id, RequestAborted));

    [HttpPost("couriers/{id:int}/activate")]
    public async Task<IActionResult> ActivateCourier(int id)
        => FromResult(await _adminService.SetCourierActiveAsync(id, isActive: true, RequestAborted));

    [HttpPost("couriers/{id:int}/deactivate")]
    public async Task<IActionResult> DeactivateCourier(int id)
        => FromResult(await _adminService.SetCourierActiveAsync(id, isActive: false, RequestAborted));

    [HttpDelete("couriers/{id:int}")]
    public async Task<IActionResult> DeleteCourier(int id)
        => FromResult(await _adminService.DeleteCourierAsync(id, RequestAborted));
}
