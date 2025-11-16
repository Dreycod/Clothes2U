using FrontBlazor.Models;

namespace FrontBlazor.Services
{
    public class AuthService : BaseGenericService
    {
        private readonly HttpClient _httpClient;

        public AuthService(HttpClient httpClient) : base(httpClient)
        {}

        public async Task<SignUpResponse?> SignUpAsync(LoginRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("signup", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("SignUp Error: " + error);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<SignUpResponse>();
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("login", request);

            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Login Error: " + error);
                return null;
            }

            return await response.Content.ReadFromJsonAsync<AuthResponse>();
        }
    }
}
