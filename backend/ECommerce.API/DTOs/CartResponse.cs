namespace ECommerce.API.DTOs;

public class CartResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public IReadOnlyList<CartItemResponse> Items { get; set; } = [];
    public int TotalItems { get; set; }
    public decimal TotalPrice { get; set; }
    public DateTime CreatedAt { get; set; }
}
