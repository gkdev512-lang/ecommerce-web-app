namespace ECommerce.API.Models;

public class Payment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid OrderId { get; set; }
    public decimal Amount { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public string Status { get; set; } = PaymentStatuses.Pending;
    public string TransactionId { get; set; } = string.Empty;
    public DateTime? PaidAt { get; set; }

    public Order? Order { get; set; }
}
