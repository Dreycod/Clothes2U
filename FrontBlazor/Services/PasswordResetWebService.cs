using System.Net.Http.Json;
using Shared.DTO.ForgotPassword;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.Services
{
    public class PasswordResetWebService : BaseGenericService
    {
        public PasswordResetWebService(HttpClient httpClient)
            : base(httpClient)
        {
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var dto = new ForgotPasswordDTO { Email = email };

            var response = await _httpClient.PostAsJsonAsync(
                "password/forgot", dto);

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> ResetPasswordAsync(string token, string newPassword)
        {
            var dto = new ResetPasswordDTO
            {
                Token = token,
                NewPassword = newPassword
            };

            var response = await _httpClient.PostAsJsonAsync(
                "password/reset", dto);

            return response.IsSuccessStatusCode;
        }
    }
}
