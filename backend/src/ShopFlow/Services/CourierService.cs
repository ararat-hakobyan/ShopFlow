using Microsoft.Extensions.Logging;
using ShopFlow.Common;
using ShopFlow.DTOModels;
using ShopFlow.Enums;
using ShopFlow.Services.Interfaces;
using ShopFlow.DAL.Repositories.Interfaces;
using ShopFlow.Mappings;

namespace ShopFlow.Services;

public sealed class CourierService : ICourierService
{
    private const int HistorySize = 10;

    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CourierService> _logger;

    public CourierService(IUnitOfWork unitOfWork, ILogger<CourierService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<CourierConsoleDto> GetConsoleAsync(
        int courierId,
        CancellationToken cancellationToken = default)
    {
        var available = await _unitOfWork.Orders.ListUnassignedAsync(cancellationToken);

        var active = await _unitOfWork.Orders.ListByCourierAsync(
            courierId,
            OrderStatus.OutForDelivery,
            cancellationToken: cancellationToken);

        var delivered = await _unitOfWork.Orders.ListByCourierAsync(
            courierId,
            OrderStatus.Delivered,
            HistorySize,
            cancellationToken);

        return new CourierConsoleDto
        {
            AvailableOrders = available.ToDtoList(),
            ActiveOrders = active.ToDtoList(),
            DeliveredOrders = delivered.ToDtoList()
        };
    }

    public async Task<Result> AcceptOrderAsync(
        int orderId,
        int courierId,
        CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId, cancellationToken);

        if (order is null)
        {
            return Result.NotFound("Order not found.");
        }

        if (order.IsAssigned)
        {
            return Result.Conflict("Another courier has already taken this order.");
        }

        if (order.Status != OrderStatus.Pending)
        {
            return Result.Conflict("Only pending orders can be accepted.");
        }

        order.AssignTo(courierId);

        if (!await _unitOfWork.TrySaveChangesAsync(cancellationToken))
        {
            _logger.LogInformation(
                "Courier {CourierId} lost the race for order {OrderId}.",
                courierId,
                orderId);

            return Result.Conflict("Another courier has already taken this order.");
        }

        _logger.LogInformation("Order {OrderId} accepted by courier {CourierId}.", orderId, courierId);

        return Result.Success($"Order #{orderId} is now yours.");
    }

    public async Task<Result> CompleteOrderAsync(
        int orderId,
        int courierId,
        CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.Orders.GetByIdAsync(orderId, cancellationToken);

        if (order is null)
        {
            return Result.NotFound("Order not found.");
        }

        if (order.CourierID != courierId)
        {
            return Result.Forbidden("This order is assigned to another courier.");
        }

        if (order.Status != OrderStatus.OutForDelivery)
        {
            return Result.Conflict("Only orders that are out for delivery can be completed.");
        }

        order.MarkAsDelivered();

        if (!await _unitOfWork.TrySaveChangesAsync(cancellationToken))
        {
            return Result.Conflict("This order was changed at the same time. Reload the page and try again.");
        }

        _logger.LogInformation("Order {OrderId} delivered by courier {CourierId}.", orderId, courierId);

        return Result.Success($"Order #{orderId} marked as delivered.");
    }
}
