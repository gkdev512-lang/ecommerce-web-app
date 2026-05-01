using System.ComponentModel.DataAnnotations;

namespace ECommerce.API.DTOs;

public class AddCartItemRequest
{
    [Required]
    public Guid ProductId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; } = 1;
}
