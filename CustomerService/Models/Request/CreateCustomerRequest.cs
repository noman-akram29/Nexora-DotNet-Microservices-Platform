using System.ComponentModel.DataAnnotations;

namespace CustomerService.Models.Request;

public class CreateCustomerRequest
{
    [Required(ErrorMessage = "Full name is required.")]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Phone number is required.")]
    [Phone(ErrorMessage = "Phone number is invalid.")]
    [MaxLength(15)]
    public string Phone { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email address is invalid.")]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Address { get; set; } = string.Empty;

    /// <summary>URL ảnh đại diện (lấy từ kết quả upload MinIO). Không bắt buộc.</summary>
    [MaxLength(500)]
    public string? AvatarUrl { get; set; }
}
