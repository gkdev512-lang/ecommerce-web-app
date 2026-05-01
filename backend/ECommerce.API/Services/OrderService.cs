using ECommerce.API.DTOs;
using ECommerce.API.Models;
using ECommerce.API.Repositories;

namespace ECommerce.API.Services;

public class OrderService : IOrderService
{
    private readonly ICartRepository _cartRepository;
    private readonly IOrderRepository _orderRepository;

    public OrderService(ICartRepository cartRepository, IOrderRepository orderRepository)
    {
        _cartRepository = cartRepository;
        _orderRepository = orderRepository;
    }

    public async Task<OrderResponse> CreateAsync(
        Guid userId,
        CreateOrderRequest request,
        CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
        if (cart is null || cart.Items.Count == 0)
        {
            throw new InvalidOperationException("Cart is empty.");
        }

        foreach (var item in cart.Items)
        {
            if (item.Product is null)
            {
                throw new InvalidOperationException("Cart contains an unavailable product.");
            }

            if (item.Quantity > item.Product.StockQuantity)
            {
                throw new InvalidOperationException($"Requested quantity exceeds available stock for {item.Product.Name}.");
            }
        }

        var orderItems = cart.Items.Select(item => new OrderItem
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            UnitPrice = item.UnitPrice
        }).ToList();

        var order = new Order
        {
            UserId = userId,
            OrderDate = DateTime.UtcNow,
            Status = OrderStatuses.Pending,
            ShippingAddress = request.ShippingAddress.Trim(),
            Items = orderItems,
            TotalAmount = orderItems.Sum(item => item.Quantity * item.UnitPrice)
        };

        var created = await _orderRepository.CreateFromCartAsync(order, cart, cancellationToken);
        return ToResponse(created);
    }

    public async Task<IReadOnlyList<OrderResponse>> GetMyOrdersAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var orders = await _orderRepository.GetByUserIdAsync(userId, cancellationToken);
        return orders.Select(ToResponse).ToList();
    }

    public async Task<OrderResponse?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default)
    {
        var order = await _orderRepository.GetByIdAsync(id, cancellationToken);
        if (order is null || order.UserId != userId)
        {
            return null;
        }

        return ToResponse(order);
    }

    private static OrderResponse ToResponse(Order order)
    {
        var items = order.Items
            .OrderBy(item => item.Product?.Name)
            .Select(item => new OrderItemResponse
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product?.Name ?? string.Empty,
                ProductDescription = item.Product?.Description ?? string.Empty,
                ProductImageUrl = item.Product?.ImageUrl ?? string.Empty,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.Quantity * item.UnitPrice
            })
            .ToList();

        return new OrderResponse
        {
            Id = order.Id,
            UserId = order.UserId,
            OrderDate = order.OrderDate,
            TotalAmount = order.TotalAmount,
            Status = order.Status,
            ShippingAddress = order.ShippingAddress,
            Items = items,
            TotalItems = items.Sum(item => item.Quantity)
        };
    }
}
