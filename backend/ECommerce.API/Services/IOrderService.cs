using ECommerce.API.DTOs;

namespace ECommerce.API.Services;

public interface IOrderService
{
    Task<OrderResponse> CreateAsync(Guid userId, CreateOrderRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<OrderResponse>> GetMyOrdersAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<OrderResponse?> GetByIdAsync(Guid userId, Guid id, CancellationToken cancellationToken = default);
}
