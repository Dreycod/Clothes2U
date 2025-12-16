using System.Text;
using System.Text.Json;
using API.DTO;
using API.DTO.Annonce;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services;
using AutoMapper;

public class SuggestionService : ISuggestionService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<SuggestionService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory; 
    private readonly IMapper _mapper;
    private readonly string _fastApiBaseUrl;
    public SuggestionService(
        IHttpClientFactory httpClientFactory, 
        ILogger<SuggestionService> logger,
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory, 
        IMapper mapper)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
        _mapper = mapper;
        _fastApiBaseUrl = configuration["FastApi:BaseUrl"] ?? "http://localhost:8001";
    }

    public async Task CalculSuggestion(int userId)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var annonceManager = scope.ServiceProvider.GetRequiredService<IAnnonceRepository<Annonce, int, FilterDTO>>();
                var annonces = await annonceManager.GetByUtilisateurFavoris(userId);
                var annonceSuggestionDTOs = _mapper.Map<List<AnnonceSuggestionDTO>>(annonces);
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(5);
                var payload = new
                {
                    user_id = userId,
                    annonces = annonceSuggestionDTOs
                };
                var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true 
                });
                _logger.LogInformation("JSON envoyé à Python:\n{Json}", json);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{_fastApiBaseUrl}/clustering/calculate", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation(
                        "Demande de calcul envoyée avec succès pour l'utilisateur {UserId} ({Count} annonces)", 
                        userId, 
                        annonceSuggestionDTOs.Count);
                }
                else
                {
                    _logger.LogWarning(
                        "Échec de l'envoi de la demande de calcul : {StatusCode}", 
                        response.StatusCode);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "Erreur lors de l'envoi de la demande de calcul pour l'utilisateur {UserId}", 
                    userId);
            }
        });

        await Task.CompletedTask;
    }
}