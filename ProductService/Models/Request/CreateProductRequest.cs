using System.ComponentModel.DataAnnotations;

namespace ProductService.Models.Request;

public class CreateProductRequest
{
    [Required(ErrorMessage = "Product name is required.")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>Mã SKU — để trống hệ thống sẽ tự động tạo</summary>
    [MaxLength(50)]
    public string? SKU { get; set; }

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

    /// <summary>Danh sách URL các ảnh sản phẩm</summary>
    public List<string> ImageUrls { get; set; } = new();
}
