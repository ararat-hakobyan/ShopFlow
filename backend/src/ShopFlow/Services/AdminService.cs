using Microsoft.Extensions.Logging;
using ShopFlow.Common;
using ShopFlow.DTOModels;
using ShopFlow.Models;
using ShopFlow.Enums;
using ShopFlow.Services.Interfaces;
using ShopFlow.DAL.Repositories.Interfaces;
using ShopFlow.Mappings;

namespace ShopFlow.Services;

public sealed class AdminService : IAdminService
{
    private const string DefaultColor = "Default";
    private const string DefaultSize = "Standard";

    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILogger<AdminService> _logger;

    public AdminService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        ILogger<AdminService> logger)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
        _logger = logger;
    }

    public async Task<AdminDashboardDto> GetDashboardAsync(
        string? search = null,
        CancellationToken cancellationToken = default)
    {
        var totalOrders = await _unitOfWork.Orders.CountAsync(cancellationToken);
        var pendingOrders = await _unitOfWork.Orders.CountByStatusAsync(OrderStatus.Pending, cancellationToken);
        var totalProducts = await _unitOfWork.Products.CountAsync(cancellationToken);
        var totalCouriers = await _unitOfWork.Couriers.CountAsync(cancellationToken);

        var orders = await _unitOfWork.Orders.SearchAsync(search, cancellationToken);
        var products = await _unitOfWork.Products.ListWithVariantsAsync(search, cancellationToken: cancellationToken);
        var categories = await _unitOfWork.Categories.ListAsync(cancellationToken);
        var couriers = await _unitOfWork.Couriers.ListAsync(cancellationToken);

        return new AdminDashboardDto
        {
            TotalOrders = totalOrders,
            PendingOrders = pendingOrders,
            TotalProducts = totalProducts,
            TotalCouriers = totalCouriers,
            Orders = orders.ToDtoList(),
            Products = products.ToDtoList(),
            Categories = categories.ToDtoList(),
            Couriers = couriers.ToDtoList(),
            SearchQuery = search
        };
    }

    public async Task<Result> AddCategoryAsync(
        AddCategoryRequest request,
        CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Categories.ExistsByNameAsync(request.Name, cancellationToken))
        {
            return Result.Conflict($"A category named \"{request.Name.Trim()}\" already exists.");
        }

        var category = new Category { Name = request.Name.Trim() };

        await _unitOfWork.Categories.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Category added.");
    }

    public async Task<Result> AddProductAsync(
        AddProductRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!await _unitOfWork.Categories.ExistsAsync(request.CategoryId, cancellationToken))
        {
            return Result.NotFound("The selected category no longer exists.");
        }

        var product = new Product
        {
            ProductName = request.ProductName.Trim(),
            CategoryID = request.CategoryId
        };

        await _unitOfWork.Products.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success($"Product \"{product.ProductName}\" added.");
    }

    public async Task<Result> AddVariantAsync(
        AddVariantRequest request,
        CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.ProductId, cancellationToken);

        if (product is null)
        {
            return Result.NotFound("The selected product no longer exists.");
        }

        var variant = new ProductVariant
        {
            ProductID = request.ProductId,
            Color = string.IsNullOrWhiteSpace(request.Color) ? DefaultColor : request.Color.Trim(),
            Size = string.IsNullOrWhiteSpace(request.Size) ? DefaultSize : request.Size.Trim(),
            Price = request.Price,
            StockQuantity = request.StockQuantity
        };

        await _unitOfWork.Products.AddVariantAsync(variant, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Variant added.");
    }

    public async Task<Result> AddCourierAsync(
        AddCourierRequest request,
        CancellationToken cancellationToken = default)
    {
        if (await _unitOfWork.Users.ExistsAsync(request.Username, request.Email, cancellationToken))
        {
            return Result.Conflict("A user with this username or email address already exists.");
        }

        var user = new User
        {
            Username = request.Username.Trim(),
            Email = request.Email.Trim(),
            Role = UserRole.Courier,
            PasswordHash = _passwordHasher.Hash(request.Password),
            Courier = new Courier
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Phone = request.Phone.Trim(),
                VehicleType = string.IsNullOrWhiteSpace(request.VehicleType)
                    ? null
                    : request.VehicleType.Trim()
            }
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Courier account created for {Username}.", user.Username);

        return Result.Success($"Courier {user.Courier.FullName} registered.");
    }

    public async Task<Result> DeleteProductAsync(int productId, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetWithVariantsAsync(productId, cancellationToken);

        if (product is null)
        {
            return Result.NotFound("Product not found.");
        }

        var variantIds = product.ProductVariants.Select(variant => variant.VariantID).ToList();

        if (await _unitOfWork.Products.HasActiveReferencesAsync(variantIds, cancellationToken))
        {
            return Result.Conflict(
                "This product is still in a basket or in an order that has not been completed yet.");
        }

        // Soft delete: the rows stay in the database so that historical orders keep their data,
        // while the global query filter hides them from the shop.
        product.IsDeleted = true;

        foreach (var variant in product.ProductVariants)
        {
            variant.IsDeleted = true;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success($"Product \"{product.ProductName}\" removed from the shop.");
    }

    public async Task<Result> DeleteCourierAsync(int courierId, CancellationToken cancellationToken = default)
    {
        var courier = await _unitOfWork.Couriers.GetByIdAsync(courierId, cancellationToken);

        if (courier is null)
        {
            return Result.NotFound("Courier not found.");
        }

        if (await _unitOfWork.Couriers.HasOrdersAsync(courierId, cancellationToken))
        {
            return Result.Conflict("This courier already has orders and cannot be deleted.");
        }

        // The courier profile shares its primary key with the user account and is removed
        // with it through the cascade configured on the relationship.
        var user = await _unitOfWork.Users.GetByIdAsync(courierId, cancellationToken);

        if (user is null)
        {
            return Result.NotFound("The account behind this courier no longer exists.");
        }

        _unitOfWork.Users.Remove(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Courier deleted.");
    }

    public async Task<Result> UpdateOrderStatusAsync(
        UpdateOrderStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(request.OrderId, cancellationToken);

        if (order is null)
        {
            return Result.NotFound("Order not found.");
        }

        if (request.Status == OrderStatus.OutForDelivery)
        {
            if (request.CourierId is null or <= 0)
            {
                return Result.Failure("Choose a courier before setting the status to \"Out for delivery\".");
            }

            var courier = await _unitOfWork.Couriers.GetByIdAsync(request.CourierId.Value, cancellationToken);

            if (courier is null)
            {
                return Result.NotFound("The selected courier no longer exists.");
            }

            order.CourierID = courier.CourierID;
        }
        else
        {
            order.CourierID = null;
        }

        order.Status = request.Status;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success($"Order #{order.OrderID} updated.");
    }

    public async Task<Result> UpdateVariantPriceAsync(
        UpdateVariantPriceRequest request,
        CancellationToken cancellationToken = default)
    {
        var variant = await _unitOfWork.Products.GetVariantAsync(request.VariantId, cancellationToken);

        if (variant is null)
        {
            return Result.NotFound("Variant not found.");
        }

        variant.Price = request.NewPrice;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Price updated.");
    }
}
