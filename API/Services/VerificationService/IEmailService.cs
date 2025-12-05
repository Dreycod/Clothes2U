namespace API.Services.Email;

public interface IEmailService
{
    Task<bool> SendVerificationEmailAsync(string email, string code);
    Task<bool> SendEmailAsync(string to, string subject, string body);
}