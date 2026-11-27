using System.ComponentModel.DataAnnotations;
using OrderService.Utils.Enum;

namespace OrderService.Models.Request;

public class CreateOrderRequest
{
    public string CustomerName { get; set; } = string.Empty;

    [Required]
    public string CustomerPhone { get; set; } = string.Empty;

    [Required]
    public string ShippingAddress { get; set; } = string.Empty;

    public PaymentMethod PaymentMethod { get; set; }
    public decimal ShippingFee { get; set; } = 0;
    public string Note { get; set; } = string.Empty;

    [Required]
    [MinLength(1, ErrorMessage = "An order must contain at least one product.")]
    public List<OrderItemRequest> Items { get; set; } = new();
}

public class OrderItemRequest
{
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be greater than 0.")]
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}