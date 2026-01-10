using FrontBlazor.Services.Interfaces;
using System.Net.Http.Json;
using Shared.DTO.Visualisation;

namespace FrontBlazor.Services
{
    public class VisualisationWebService : IVisualisationService
    {
        private readonly HttpClient _httpClient;
        private readonly IAuthService _authService;

        public VisualisationWebService(HttpClient httpClient, IAuthService authService)
        {
            _httpClient = httpClient;
            _authService = authService;
        }

        public async Task CreateVisualisationAsync(int annonceId)
        {
            var user = await _authService.GetCurrentUserAsync();
            if (user == null) return;

            var dto = new VisualisationCreateDTO
            {
                UtilisateurId = user.UtilisateurId,
                AnnonceId = annonceId
            };

            await _httpClient.PostAsJsonAsync("Visualisation", dto);
        }
    }
}
