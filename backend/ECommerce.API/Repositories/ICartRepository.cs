using ECommerce.API.Models;

namespace ECommerce.API.Repositories;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Cart> GetOrCreateAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddItemAsync(CartItem cartItem, CancellationToken cancellationToken = default);
    Task UpdateItemAsync(CartItem cartItem, CancellationToken cancellationToken = default);
    Task RemoveItemAsync(CartItem cartItem, CancellationToken cancellationToken = default);
    Task ClearAsync(Cart cart, CancellationToken cancellationToken = default);
}
