using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using NotificationService.Services.Interface;

namespace NotificationService.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;
    private readonly ILogger<EmailService> _logger;
    private readonly IWebHostEnvironment _env;

    public EmailService(IConfiguration config, ILogger<EmailService> logger, IWebHostEnvironment env)
    {
        _config = config;
        _logger = logger;
        _env = env;
    }

    public async Task SendOtpEmailAsync(string toEmail, string otpCode, string otpType)
    {
        var subject = otpType == "REGISTER"
            ? "🔐 Nexora Account Verification Code"
            : "🔑 Nexora Password Reset Code";

        var body = await BuildEmailHtmlAsync(otpCode, otpType);

        var fromEmail = _config["Smtp:FromEmail"];

        if (string.IsNullOrWhiteSpace(fromEmail))

            throw new InvalidOperationException("SMTP sender email is not configured.");


        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(

            _config["Smtp:FromName"] ?? "Nexora Support",

            fromEmail));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = body };
        message.Body = bodyBuilder.ToMessageBody();

        var smtpHost = _config["Smtp:Host"];
        if (string.IsNullOrWhiteSpace(smtpHost))
            throw new InvalidOperationException("SMTP host is not configured.");

        if (!int.TryParse(_config["Smtp:Port"] ?? "587", out var smtpPort) ||
            smtpPort < 1 || smtpPort > 65535)
            throw new InvalidOperationException("SMTP port is invalid.");

        var securityValue = _config["Smtp:Security"] ?? "StartTls";
        if (!Enum.TryParse<SecureSocketOptions>(securityValue, true, out var security) ||
            (security != SecureSocketOptions.None &&
             security != SecureSocketOptions.StartTls &&
             security != SecureSocketOptions.SslOnConnect))
            throw new InvalidOperationException("SMTP security must be None, StartTls, or SslOnConnect.");

        var smtpUsername = _config["Smtp:Username"];
        var smtpPassword = _config["Smtp:Password"];

        if (string.IsNullOrWhiteSpace(smtpUsername) != string.IsNullOrWhiteSpace(smtpPassword))
            throw new InvalidOperationException("SMTP username and password must either both be configured or both be empty.");

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(smtpHost, smtpPort, security);

        if (!string.IsNullOrWhiteSpace(smtpUsername))
            await smtp.AuthenticateAsync(smtpUsername, smtpPassword!);

        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(true);

        _logger.LogInformation("OTP email ({Type}) sent to {Email}", otpType, toEmail);
    }

    private async Task<string> BuildEmailHtmlAsync(string otpCode, string otpType)
    {
        var templatePath = Path.Combine(_env.ContentRootPath, "Templates", "otp_email.html");

        if (!File.Exists(templatePath))
            throw new FileNotFoundException($"Email template not found at: {templatePath}");

        var (title, subtitle, action) = otpType == "REGISTER"
            ? (
                "Verify Your Account",
                "Welcome to <em>Nexora</em>! Please use the verification code below to complete your account setup.",
                "verify your account"
              )
            : (
                "Reset Your Password",
                "We received a request to <strong>reset the password</strong> for your <em>Nexora</em> account. Use the code below to continue.",
                "reset your password"
              );

        var html = await File.ReadAllTextAsync(templatePath);
        html = html.Replace("{{TITLE}}", title);
        html = html.Replace("{{SUBTITLE}}", subtitle);
        html = html.Replace("{{OTP_CODE}}", otpCode);
        html = html.Replace("{{ACTION}}", action);

        return html;
    }
}
