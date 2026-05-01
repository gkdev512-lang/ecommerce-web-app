using ECommerce.API.Data;
using ECommerce.API.Models;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.API.Repositories;

public class CartRepository : ICartRepository
{
    private readonly ApplicationDbContext _dbContext;

    public CartRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await CartWithItems()
            .FirstOrDefaultAsync(cart => cart.UserId == userId, cancellationToken);
    }

    public async Task<Cart> GetOrCreateAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cart = await GetByUserIdAsync(userId, cancellationToken);
        if (cart is not null)
        {
            return cart;
        }

        cart = new Cart
        {
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _dbContext.Carts.AddAsync(cart, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return cart;
    }

    public async Task AddItemAsync(CartItem cartItem, CancellationToken cancellationToken = default)
    {
        await _dbContext.CartItems.AddAsync(cartItem, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateItemAsync(CartItem cartItem, CancellationToken cancellationToken = default)
    {
        _dbContext.CartItems.Update(cartItem);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveItemAsync(CartItem cartItem, CancellationToken cancellationToken = default)
    {
        _dbContext.CartItems.Remove(cartItem);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task ClearAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        _dbContext.CartItems.RemoveRange(cart.Items);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    private IQueryable<Cart> CartWithItems()
    {
        return _dbContext.Carts
            .Include(cart => cart.Items)
                .ThenInclude(item => item.Product);
    }
}
