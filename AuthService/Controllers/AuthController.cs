using AuthService.Models.Request;
using AuthService.Models.Response;
using AuthService.Services.Interface;
using CommonService.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers;

[Route("api/v1/auth")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly ITokenService _tokenService;
    private readonly IAuthService  _authService;

    public AuthController(ITokenService tokenService, IAuthService authService)
    {
        _tokenService = tokenService;
        _authService  = authService;
    }

    // POST /api/v1/auth/login
    [HttpPost("login")]
    [AllowAnonymous]
    [ApiMessage("Login successful.")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDTO request)
    {
        var result = await _authService.LoginAsync(request);
        return Ok(result); // cookie đã được Service set qua IHttpContextAccessor
    }

    // POST /api/v1/auth/register
    [HttpPost("register")]
    [AllowAnonymous]
    [ApiMessage("Registration successful.")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDTO request)
    {
        var result = await _authService.RegisterAsync(request);
        return StatusCode(201, result);
    }

    // GET /api/v1/auth/account
    [HttpGet("account")]
    [Authorize]
    [ApiMessage("Account information retrieved successfully.")]
    public async Task<IActionResult> GetAccount()
    {
        var email = _tokenService.GetCurrentUserEmail();
        if (string.IsNullOrEmpty(email)) return Unauthorized();

        // Lấy token hiện tại từ header Authorization: Bearer xxx
        var currentToken = Request.Headers["Authorization"]
            .ToString().Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase).Trim();

        var result = await _authService.GetAccountAsync(email);
        result.AccessToken = string.IsNullOrEmpty(currentToken) ? null : currentToken;
        return Ok(result);
    }

    // PUT /api/v1/auth/email
    [HttpPut("email")]
    [Authorize]
    [ApiMessage("Security information updated successfully.")]
    public async Task<IActionResult> UpdateEmail([FromBody] UpdateEmailRequestDTO request)
    {
        // Lấy ID từ token 
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        await _authService.UpdateEmailAsync(userId, request);
        return Ok(new { message = "Email and password updated successfully." });
    }

    // GET /api/v1/auth/refresh
    [HttpGet("refresh")]
    [AllowAnonymous]
    [ApiMessage("Token refreshed successfully.")]
    public async Task<IActionResult> RefreshToken()
    {
        var refreshToken = Request.Cookies["refresh_token"];
        if (string.IsNullOrEmpty(refreshToken))
            return BadRequest(new { message = "Refresh token not found." });

        var result = await _authService.RefreshTokenAsync(refreshToken);
        return Ok(result); // cookie mới đã được Service set
    }

    // POST /api/v1/auth/logout
    [HttpPost("logout")]
    [Authorize]
    [ApiMessage("Logout successful.")]
    public async Task<IActionResult> Logout()
    {
        var email = _tokenService.GetCurrentUserEmail();
        if (!string.IsNullOrEmpty(email))
            await _authService.LogoutAsync(email); // Service tự xóa cookie

        return Ok(new { message = "Logout successful." });
    }

    // PATCH /api/v1/auth/change-password
    [HttpPatch("change-password")]
    [Authorize]
    [ApiMessage("Password changed successfully.")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
    {
        var userIdString = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
            return Unauthorized();

        await _authService.ChangePasswordAsync(userId, request);
        return Ok(new { message = "Password changed successfully." });
    }

    // ── OTP Endpoints ─────────────────────────────────────────────────────────

    // POST /api/v1/auth/verify-email
    [HttpPost("verify-email")]
    [AllowAnonymous]
    [ApiMessage("Email verification successful.")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        await _authService.VerifyEmailAsync(request);
        return Ok(new { message = "Your account has been verified successfully. You can now sign in." });
    }

    // POST /api/v1/auth/send-otp-reset
    [HttpPost("send-otp-reset")]
    [AllowAnonymous]
    [ApiMessage("Password reset OTP sent successfully.")]
    public async Task<IActionResult> SendOtpReset([FromBody] SendOtpRequest request)
    {
        await _authService.SendOtpResetPasswordAsync(request);
        return Ok(new { message = $"Password reset OTP has been sent to {request.Email}." });
    }

    // POST /api/v1/auth/reset-password
    [HttpPost("reset-password")]
    [AllowAnonymous]
    [ApiMessage("Password reset successful.")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        await _authService.ResetPasswordAsync(request);
        return Ok(new { message = "Your password has been reset successfully. Please sign in with your new password." });
    }

    // POST /api/v1/auth/resend-otp
    [HttpPost("resend-otp")]
    [AllowAnonymous]
    [ApiMessage("OTP resent successfully.")]
    public async Task<IActionResult> ResendOtp([FromBody] SendOtpRequest request, [FromQuery] string otpType = "REGISTER")
    {
        if (otpType != "REGISTER" && otpType != "RESET_PASSWORD")
            return BadRequest(new { message = "Invalid otpType. Accepted values: REGISTER | RESET_PASSWORD." });

        await _authService.ResendOtpAsync(request, otpType);
        return Ok(new { message = $"OTP has been resent to {request.Email}." });
    }
}
