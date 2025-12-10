using Microsoft.Extensions.Configuration;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace API.Services.VerificationSrvceV2
{
    public class TwilioSmsService : ISmsService
    {
        private readonly IConfiguration _config;

        public TwilioSmsService(IConfiguration config)
        {
            _config = config;

            TwilioClient.Init(
                _config["Twilio:AccountSid"],
                _config["Twilio:AuthToken"]
            );
        }

        public async Task SendAsync(string phoneNumber, string message)
        {
            await MessageResource.CreateAsync(
                body: message,
                from: new Twilio.Types.PhoneNumber(_config["Twilio:FromPhone"]),
                to: new Twilio.Types.PhoneNumber(phoneNumber)
            );
        }
    }
}
