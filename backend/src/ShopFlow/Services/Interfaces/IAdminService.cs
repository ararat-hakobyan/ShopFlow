using ShopFlow.Common;
using ShopFlow.DTOModels;

namespace ShopFlow.Services.Interfaces;

public interface IAdminService
{
    Task<AdminDashboardDto> GetDashboardAsync(string? search = null, CancellationToken cancellationToken = default);

    Task<Result> AddCategoryAsync(AddCategoryRequest request, CancellationToken cancellationToken = default);

    Task<Result> AddProductAsync(AddProductRequest request, CancellationToken cancellationToken = default);

    Task<Result> AddVariantAsync(AddVariantRequest request, CancellationToken cancellationToken = default);

    Task<Result> AddCourierAsync(AddCourierRequest request, CancellationToken cancellationToken = default);

    Task<Result> DeleteProductAsync(int productId, CancellationToken cancellationToken = default);

    Task<Result> DeleteCourierAsync(int courierId, CancellationToken cancellationToken = default);

    Task<Result> SetCourierActiveAsync(int courierId, bool isActive, CancellationToken cancellationToken = default);

    Task<Result> UpdateOrderStatusAsync(UpdateOrderStatusRequest request, CancellationToken cancellationToken = default);

    Task<Result> UpdateVariantPriceAsync(UpdateVariantPriceRequest request, CancellationToken cancellationToken = default);
}
