using System.Text;
using System.Text.Json;
using Shared.DTO;
using Shared.DTO.Annonce;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services;
using AutoMapper;

public class SuggestionService : ISuggestionService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IAnnonceRepository<Annonce, int, FilterDTO> _annonceManager;
    private readonly ILogger<SuggestionService> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory; 
    private readonly IMapper _mapper;
    private readonly string _fastApiBaseUrl;
    private readonly IClusterRepository _annoncePreferenceManager;
    public SuggestionService(
        IHttpClientFactory httpClientFactory, 
        IClusterRepository  annoncePreferenceManager,
        IAnnonceRepository<Annonce, int, FilterDTO> annonceRepository,
        ILogger<SuggestionService> logger,
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory, 
        
        IMapper mapper)
    {
        _httpClientFactory = httpClientFactory;
        _annonceManager = annonceRepository;
        _logger = logger;
        _annoncePreferenceManager = annoncePreferenceManager;
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
                var clusterRepository = scope.ServiceProvider.GetRequiredService<IClusterRepository>();
                var annonces = await annonceManager.GetByUtilisateurFavoris(userId);
                var annonceSuggestionDTOs = _mapper.Map<List<AnnonceSuggestionDTO>>(annonces);
                var payload = new
                {
                    userId = userId,
                    annonces = annonceSuggestionDTOs
                };
                
                var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = true 
                });
                
                _logger.LogInformation("📤 JSON envoyé à Python:\n{Json}", json);
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(30);
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{_fastApiBaseUrl}/clustering/calculate", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    
                    _logger.LogInformation("📥 Réponse reçue de Python:\n{Response}", responseContent);
                    var clusteringResult = JsonSerializer.Deserialize<ClusteringResponseDTO>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            PropertyNameCaseInsensitive = true
                        });

                    if (clusteringResult != null && clusteringResult.Success)
                    {
                        await clusterRepository.SaveClustersAsync(clusteringResult);
                        
                        _logger.LogInformation(
                            "✅ Clustering terminé et sauvegardé pour l'utilisateur {UserId} " +
                            "({NbAnnonces} annonces, {NbCategories} catégories)",
                            userId,
                            clusteringResult.NbAnnoncesTotal,
                            clusteringResult.NbCategories);
                    }
                    else
                    {
                        _logger.LogWarning("⚠️ Le clustering a échoué pour l'utilisateur {UserId}", userId);
                    }
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    _logger.LogWarning(
                        "❌ Échec de l'appel Python : {StatusCode}\n{Error}", 
                        response.StatusCode,
                        errorContent);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, 
                    "❌ Erreur lors du calcul des suggestions pour l'utilisateur {UserId}", 
                    userId);
            }
        });

        await Task.CompletedTask;
    }

    public async Task<IEnumerable<Annonce>> GetRecommandations(int userId)
    {
        IEnumerable<Annonce> annonces = await _annonceManager.GetActiveAnnonces();
        IEnumerable<AnnoncePreferenceUtilisateur> clusters = await _annoncePreferenceManager.GetAllAsync();
        
        throw new NotImplementedException();
    }
}