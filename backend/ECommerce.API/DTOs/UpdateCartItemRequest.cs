using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.DTOs;

public class UpdateCartItemRequest
{
    [Required]
    public Guid CartItemId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }
}
