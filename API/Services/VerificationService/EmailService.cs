using System.Net;
using System.Net.Mail;

namespace API.Services.Email;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> SendVerificationEmailAsync(string email, string code)
    {
        var subject = "Code de vérification Clothes2U";
        var body = $@"
            <html>
            <body>
                <h2>Vérification de votre compte</h2>
                <p>Votre code de vérification est :</p>
                <h1 style='color: #4CAF50; font-size: 36px; letter-spacing: 5px;'>{code}</h1>
                <p>Ce code expire dans 10 minutes.</p>
                <p>Si vous n'avez pas demandé ce code, ignorez cet email.</p>
            </body>
            </html>
        ";

        return await SendEmailAsync(email, subject, body);
    }

    public async Task<bool> SendEmailAsync(string to, string subject, string body)
    {
        try
        {
            var smtpHost = _configuration["Email:SmtpHost"];
            var smtpPort = int.Parse(_configuration["Email:SmtpPort"] ?? "587");
            var smtpUser = _configuration["Email:SmtpUser"];
            var smtpPassword = _configuration["Email:SmtpPassword"];
            var fromEmail = _configuration["Email:FromEmail"];
            var fromName = _configuration["Email:FromName"];

            using var client = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(smtpUser, smtpPassword),
                EnableSsl = true
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            };
            mailMessage.To.Add(to);

            await client.SendMailAsync(mailMessage);
            _logger.LogInformation("Email envoyé avec succès à {Email}", to);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'envoi de l'email à {Email}", to);
            return false;
        }
    }
}