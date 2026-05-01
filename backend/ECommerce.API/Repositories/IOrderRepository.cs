using ECommerce.API.Models;

namespace ECommerce.API.Repositories;

public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Order>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Order> CreateFromCartAsync(Order order, Cart cart, CancellationToken cancellationToken = default);
}
