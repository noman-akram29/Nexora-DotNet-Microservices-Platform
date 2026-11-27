using System.ComponentModel.DataAnnotations;

namespace ProductService.Models.Request;

public class UpdateProductRequest
{
    [Required(ErrorMessage = "Product name is required.")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "SKU is required.")]
    [MaxLength(50)]
    public string SKU { get; set; } = string.Empty;

    [MaxLength(5000)]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Category is required.")]
    public Guid CategoryId { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Purchase price must be greater than or equal to 0.")]
    public decimal CostPrice { get; set; } = 0;

    [Required(ErrorMessage = "Sale price is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Sale price must be greater than or equal to 0.")]
    public decimal Price { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Stock quantity must be greater than or equal to 0.")]
    public int StockQuantity { get; set; } = 0;

    [MaxLength(50)]
    public string Unit { get; set; } = "piece";

    [Range(0, int.MaxValue)]
    public int LowStockThreshold { get; set; } = 5;

    [MaxLength(500)]
    public string ThumbnailUrl { get; set; } = string.Empty;

    public List<string> ImageUrls { get; set; } = new();
}
