namespace API.Services.VerificationSrvceV2
{
    public interface IEmailService
    {
        Task SendAsync(string to, string subject, string body);
        Task SendHtmlAsync(string to, string subject, string htmlBody);
    }
}
