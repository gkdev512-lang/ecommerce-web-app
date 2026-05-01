using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.DTOs;

public class CreateOrderRequest
{
    [Required]
    [MaxLength(1000)]
    public string ShippingAddress { get; set; } = string.Empty;
}
