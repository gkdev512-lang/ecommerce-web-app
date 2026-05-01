using ECommerce.API.DTOs;

namespace ECommerce.API.Services;

public interface ICartService
{
    Task<CartResponse> GetAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<CartResponse> AddAsync(Guid userId, AddCartItemRequest request, CancellationToken cancellationToken = default);
    Task<CartResponse> UpdateAsync(Guid userId, UpdateCartItemRequest request, CancellationToken cancellationToken = default);
    Task<bool> RemoveAsync(Guid userId, Guid cartItemId, CancellationToken cancellationToken = default);
    Task<CartResponse> ClearAsync(Guid userId, CancellationToken cancellationToken = default);
}
