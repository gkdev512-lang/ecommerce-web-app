using ECommerce.API.DTOs;
using ECommerce.API.Models;
using ECommerce.API.Repositories;

namespace ECommerce.API.Services;

public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CartService(ICartRepository cartRepository, IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<CartResponse> GetAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetOrCreateAsync(userId, cancellationToken);
        return ToResponse(cart);
    }

    public async Task<CartResponse> AddAsync(Guid userId, AddCartItemRequest request, CancellationToken cancellationToken = default)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            throw new KeyNotFoundException("Product not found.");
        }

        var cart = await _cartRepository.GetOrCreateAsync(userId, cancellationToken);
        var existingItem = cart.Items.FirstOrDefault(item => item.ProductId == request.ProductId);
        var requestedQuantity = request.Quantity + (existingItem?.Quantity ?? 0);
        if (requestedQuantity > product.StockQuantity)
        {
            throw new InvalidOperationException("Requested quantity exceeds available stock.");
        }

        if (existingItem is null)
        {
            var cartItem = new CartItem
            {
                CartId = cart.Id,
                ProductId = product.Id,
                Quantity = request.Quantity,
                UnitPrice = product.Price
            };

            await _cartRepository.AddItemAsync(cartItem, cancellationToken);
        }
        else
        {
            existingItem.Quantity += request.Quantity;
            existingItem.UnitPrice = product.Price;
            await _cartRepository.UpdateItemAsync(existingItem, cancellationToken);
        }

        var updatedCart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
        return ToResponse(updatedCart ?? cart);
    }

    public async Task<CartResponse> UpdateAsync(Guid userId, UpdateCartItemRequest request, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetOrCreateAsync(userId, cancellationToken);
        var cartItem = cart.Items.FirstOrDefault(item => item.Id == request.CartItemId);
        if (cartItem is null)
        {
            throw new KeyNotFoundException("Cart item not found.");
        }

        if (cartItem.Product is not null && request.Quantity > cartItem.Product.StockQuantity)
        {
            throw new InvalidOperationException("Requested quantity exceeds available stock.");
        }

        cartItem.Quantity = request.Quantity;
        await _cartRepository.UpdateItemAsync(cartItem, cancellationToken);

        var updatedCart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
        return ToResponse(updatedCart ?? cart);
    }

    public async Task<bool> RemoveAsync(Guid userId, Guid cartItemId, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetOrCreateAsync(userId, cancellationToken);
        var cartItem = cart.Items.FirstOrDefault(item => item.Id == cartItemId);
        if (cartItem is null)
        {
            return false;
        }

        await _cartRepository.RemoveItemAsync(cartItem, cancellationToken);
        return true;
    }

    public async Task<CartResponse> ClearAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cart = await _cartRepository.GetOrCreateAsync(userId, cancellationToken);
        await _cartRepository.ClearAsync(cart, cancellationToken);

        var updatedCart = await _cartRepository.GetByUserIdAsync(userId, cancellationToken);
        return ToResponse(updatedCart ?? cart);
    }

    private static CartResponse ToResponse(Cart cart)
    {
        var items = cart.Items
            .OrderBy(item => item.Product?.Name)
            .Select(item => new CartItemResponse
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

        return new CartResponse
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Items = items,
            TotalItems = items.Sum(item => item.Quantity),
            TotalPrice = items.Sum(item => item.TotalPrice),
            CreatedAt = cart.CreatedAt
        };
    }
}
