using Microsoft.Extensions.Logging;
using ShopFlow.Common;
using ShopFlow.DTOModels;
using ShopFlow.Models;
using ShopFlow.Enums;
using ShopFlow.Services.Interfaces;
using ShopFlow.DAL.Repositories.Interfaces;
using ShopFlow.Mappings;

namespace ShopFlow.Services;

public sealed class ShopService : IShopService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<ShopService> _logger;

    public ShopService(IUnitOfWork unitOfWork, ILogger<ShopService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<StorefrontDto>> GetStorefrontAsync(
        int userId,
        int? categoryId = null,
        int? productId = null,
        PageRequest? orderPaging = null,
        CancellationToken cancellationToken = default)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);

        if (user is null)
        {
            return Result<StorefrontDto>.NotFound("Account not found.");
        }

        var categories = await _unitOfWork.Categories.ListAsync(cancellationToken);
        var products = await _unitOfWork.Products.ListWithVariantsAsync(
            categoryId: categoryId,
            cancellationToken: cancellationToken);
        var basket = await _unitOfWork.Baskets.GetWithItemsAsync(userId, cancellationToken);
        orderPaging ??= new PageRequest();

        var orders = await _unitOfWork.Orders.ListByUserAsync(
            userId,
            orderPaging.Page,
            orderPaging.PageSize,
            cancellationToken);

        var selectedVariants = new List<ProductVariantDto>();

        if (productId.HasValue)
        {
            var variants = await _unitOfWork.Products.ListVariantsAsync(productId.Value, cancellationToken);
            selectedVariants = variants.ToDtoList();
        }

        var storefront = new StorefrontDto
        {
            UserId = user.UserID,
            Username = user.Username,
            Email = user.Email,
            MemberSince = user.CreatedAt,
            Categories = categories.ToDtoList(),
            Products = products.ToDtoList(),
            Basket = basket.ToDto(),
            Orders = orders.Map(order => order.ToDto()),
            SelectedCategoryId = categoryId,
            SelectedProductId = productId,
            SelectedProductVariants = selectedVariants
        };

        return Result<StorefrontDto>.Success(storefront);
    }

    public async Task<Result> AddToCartAsync(
        int userId,
        AddToCartRequest request,
        CancellationToken cancellationToken = default)
    {
        var variant = await _unitOfWork.Products.GetVariantAsync(request.VariantId, cancellationToken);

        if (variant is null)
        {
            return Result.NotFound("This product variant is no longer available.");
        }

        var basket = await _unitOfWork.Baskets.GetOrCreateAsync(userId, cancellationToken);
        var existingItem = basket.BasketItems.FirstOrDefault(item => item.VariantID == request.VariantId);
        var quantityAfterwards = (existingItem?.Quantity ?? 0) + request.Quantity;

        if (!variant.IsInStock(quantityAfterwards))
        {
            return Result.Conflict(
                $"Only {variant.StockQuantity} item(s) of this variant are in stock.");
        }

        if (existingItem is null)
        {
            basket.BasketItems.Add(new BasketItem
            {
                VariantID = variant.VariantID,
                Quantity = request.Quantity
            });
        }
        else
        {
            existingItem.Quantity = quantityAfterwards;
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Added to your basket.");
    }

    public async Task<Result> RemoveFromCartAsync(
        int userId,
        RemoveFromCartRequest request,
        CancellationToken cancellationToken = default)
    {
        var basket = await _unitOfWork.Baskets.GetWithItemsAsync(userId, cancellationToken);
        var item = basket?.BasketItems.FirstOrDefault(basketItem => basketItem.VariantID == request.VariantId);

        if (item is null)
        {
            return Result.NotFound("This item is not in your basket.");
        }

        _unitOfWork.Baskets.RemoveItem(item);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success("Item removed from your basket.");
    }

    public async Task<Result<int>> PlaceOrderAsync(int userId, CancellationToken cancellationToken = default)
    {
        var basket = await _unitOfWork.Baskets.GetWithItemsAsync(userId, cancellationToken);

        if (basket is null || basket.BasketItems.Count == 0)
        {
            return Result<int>.Failure("Your basket is empty.");
        }

        var outOfStock = basket.BasketItems
            .Where(item => item.Variant is null || !item.Variant.IsInStock(item.Quantity))
            .Select(item => item.Variant?.Product?.ProductName ?? "an unavailable product")
            .ToList();

        if (outOfStock.Count > 0)
        {
            return Result<int>.Conflict(
                $"Not enough stock for: {string.Join(", ", outOfStock)}.");
        }

        var order = new Order { UserID = userId };

        foreach (var item in basket.BasketItems)
        {
            order.OrderDetails.Add(new OrderDetail
            {
                VariantID = item.VariantID,
                Quantity = item.Quantity,
                PriceAtPurchase = item.Variant.Price
            });

            item.Variant.ReduceStock(item.Quantity);
        }

        order.RecalculateTotal();

        await _unitOfWork.Orders.AddAsync(order, cancellationToken);
        _unitOfWork.Baskets.RemoveItems(basket.BasketItems.ToList());

        if (!await _unitOfWork.TrySaveChangesAsync(cancellationToken))
        {
            _logger.LogInformation("Order by user {UserId} lost a stock race and was not placed.", userId);

            return Result<int>.Conflict(
                "The stock of one of your items changed while the order was being placed. " +
                "Check your basket and try again.");
        }

        _logger.LogInformation("Order {OrderId} placed by user {UserId}.", order.OrderID, userId);

        return Result<int>.Success(order.OrderID, $"Order #{order.OrderID} has been placed.");
    }

    public async Task<Result<OrderDto>> GetOrderForUserAsync(
        int userId,
        int orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetWithLinesAsync(orderId, cancellationToken);

        if (order is null)
        {
            return Result<OrderDto>.NotFound("Order not found.");
        }

        // Without this check any signed-in customer could read somebody else's order
        // just by changing the id in the address bar.
        if (order.UserID != userId)
        {
            return Result<OrderDto>.Forbidden("This order belongs to another account.");
        }

        return Result<OrderDto>.Success(order.ToDto());
    }

    public async Task<Result> CancelOrderAsync(
        int userId,
        int orderId,
        CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetForUpdateWithLinesAsync(orderId, cancellationToken);

        if (order is null)
        {
            return Result.NotFound("Order not found.");
        }

        if (order.UserID != userId)
        {
            return Result.Forbidden("This order belongs to another account.");
        }

        if (!order.CanChangeTo(OrderStatus.Cancelled))
        {
            return Result.Conflict("Only pending orders can be cancelled.");
        }

        order.Cancel();

        if (!await _unitOfWork.TrySaveChangesAsync(cancellationToken))
        {
            return Result.Conflict("This order was changed at the same time. Reload the page and try again.");
        }

        _logger.LogInformation("Order {OrderId} cancelled by user {UserId}.", orderId, userId);

        return Result.Success($"Order #{orderId} has been cancelled.");
    }
}
