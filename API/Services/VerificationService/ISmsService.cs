namespace API.Services.SMS;

public interface ISmsService
{
    Task<bool> SendVerificationSmsAsync(string phoneNumber, string code);
    Task<bool> SendSmsAsync(string phoneNumber, string message);
}