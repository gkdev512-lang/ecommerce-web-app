namespace ECommerce.API.DTOs.Admin;

public class DashboardResponse
{
    public int TotalUsers { get; set; }
    public int TotalProducts { get; set; }
    public int TotalCategories { get; set; }
    public int TotalOrders { get; set; }
    public decimal TotalRevenue { get; set; }
    public int PendingOrders { get; set; }
    public int CompletedPayments { get; set; }

    public List<LatestOrderDto> LatestOrders { get; set; } = new();
}

public class LatestOrderDto
{
    public Guid OrderId { get; set; }
    public string CustomerEmail { get; set; } = string.Empty;
    public decimal TotalAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
}