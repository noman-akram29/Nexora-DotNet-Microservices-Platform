using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.Request;

/// <summary>Admin tạo tài khoản mới thủ công.</summary>
public class CreateUserRequest
{
    [Required(ErrorMessage = "Username is required.")]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is invalid.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Password is required.")]
    [MinLength(6, ErrorMessage = "Password must be at least 6 characters long.")]
    public string Password { get; set; } = string.Empty;

    /// <summary>Nếu null → gán Role mặc định 'USER'.</summary>
    public Guid? RoleId { get; set; }
}
