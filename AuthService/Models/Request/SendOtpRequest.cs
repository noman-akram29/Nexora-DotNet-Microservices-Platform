using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.Request;

public class SendOtpRequest
{
    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Email is invalid.")]
    public string Email { get; set; } = string.Empty;
}
