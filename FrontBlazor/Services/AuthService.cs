using FrontBlazor.Models;

namespace FrontBlazor.Services
{
    public class AuthService : BaseGenericService
    {
        public AuthService(HttpClient httpClient) : base(httpClient) {}

        public async Task<SignUpResponse?> SignUpAsync(LoginRequest request)
        {
            // works, waiting for role id bug fix
            var response = await _httpClient.PostAsJsonAsync("Login/signup", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("SignUp Error: " + error);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<SignUpResponse>();
        }

        public async Task<LoginResponse?> LoginAsync(LoginRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("login", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Login Error: " + error);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<LoginResponse>();
        }
    }
}
