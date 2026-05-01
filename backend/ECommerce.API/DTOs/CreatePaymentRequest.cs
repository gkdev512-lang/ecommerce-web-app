using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.DTOs;

public class CreatePaymentRequest
{
    [Required]
    public Guid OrderId { get; set; }

    [Required]
    [MaxLength(100)]
    public string PaymentMethod { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
}
