using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.Request;

/// <summary>Người dùng tự đổi mật khẩu — yêu cầu xác nhận mật khẩu cũ.</summary>
public class ChangePasswordRequest
{
    [Required(ErrorMessage = "Current password is required.")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "New password is required.")]
    [MinLength(6, ErrorMessage = "New password must be at least 6 characters long.")]
    public string NewPassword { get; set; } = string.Empty;
}
