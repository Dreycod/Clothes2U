using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace API.Services.SMS;

public class SmsService : ISmsService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmsService> _logger;

    public SmsService(IConfiguration configuration, ILogger<SmsService> logger)
    {
        _configuration = configuration;
        _logger = logger;

        // Initialiser Twilio
        var accountSid = _configuration["Twilio:AccountSid"];
        var authToken = _configuration["Twilio:AuthToken"];
        TwilioClient.Init(accountSid, authToken);
    }

    public async Task<bool> SendVerificationSmsAsync(string phoneNumber, string code)
    {
        var message = $"Votre code de vérification Clothes2U est : {code}. Ce code expire dans 10 minutes.";
        return await SendSmsAsync(phoneNumber, message);
    }

    public async Task<bool> SendSmsAsync(string phoneNumber, string message)
    {
        try
        {
            var fromNumber = _configuration["Twilio:PhoneNumber"];

            var messageResource = await MessageResource.CreateAsync(
                body: message,
                from: new PhoneNumber(fromNumber),
                to: new PhoneNumber(phoneNumber)
            );

            _logger.LogInformation("SMS envoyé avec succès à {PhoneNumber}. SID: {Sid}", 
                phoneNumber, messageResource.Sid);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'envoi du SMS à {PhoneNumber}", phoneNumber);
            return false;
        }
    }
}