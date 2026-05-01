namespace ECommerce.API.DTOs;

public class OrderResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime OrderDate { get; set; }
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public string ShippingAddress { get; set; } = string.Empty;
    public IReadOnlyList<OrderItemResponse> Items { get; set; } = [];
    public int TotalItems { get; set; }
}
