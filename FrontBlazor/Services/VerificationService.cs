using System.Net.Http.Json;
using Shared.DTO.Verification;
using FrontBlazor.Services.GenericService;
using Shared.Enums;

namespace FrontBlazor.Services
{
    public class VerificationService : BaseGenericService
    {
        public VerificationService(HttpClient httpClient)
       : base(httpClient)
        {
        }

        public async Task<VerificationResponse?> SendCodeAsync(
         VerificationType type,
         string? phoneNumber)
        {
            var content = JsonContent.Create(new SendVerificationCodeRequest
            {
                Type = type,
                PhoneNumber = phoneNumber
            });

            var response = await PostWithCredentialsAsync(
                "verification/send",
                content
            );

            if (!response.IsSuccessStatusCode)
            {
                return new VerificationResponse
                {
                    Success = false,
                    Message = "Erreur lors de l’envoi du code"
                };
            }

            return await response.Content.ReadFromJsonAsync<VerificationResponse>();
        }


        public async Task<VerificationResponse?> VerifyCodeAsync(string code, VerificationType type)
        {
            var content = JsonContent.Create(new VerifyCodeRequest
            {
                Code = code,
                Type = type
            });

            var response = await PostWithCredentialsAsync(
                "verification/verify",
                content
            );

            return await response.Content.ReadFromJsonAsync<VerificationResponse>();
        }
    }
}
