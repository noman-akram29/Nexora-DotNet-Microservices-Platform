using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.Request;

public class VerifyEmailRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is invalid.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "OTP code is required.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "OTP code must be exactly 6 characters long.")]
    public string OtpCode { get; set; } = string.Empty;
}
