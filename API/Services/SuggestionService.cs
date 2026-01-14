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
    private readonly IAnnonceExtensionService _annonceExtensionService;
    private readonly ICurrentUserService _currentUserService;
    private readonly string _fastApiBaseUrl;
    private readonly IClusterRepository _annoncePreferenceManager;
    private readonly IBloqueRepository<Bloque, int>  _bloqueManager;
    public SuggestionService(
        IHttpClientFactory httpClientFactory, 
        IClusterRepository  annoncePreferenceManager,
        IAnnonceExtensionService annonceExtensionService,
        IAnnonceRepository<Annonce, int, FilterDTO> annonceRepository,
        ICurrentUserService currentUserService,
        ILogger<SuggestionService> logger,
        IBloqueRepository<Bloque, int> bloqueManager,
        IConfiguration configuration,
        IServiceScopeFactory serviceScopeFactory, 
        
        IMapper mapper)
    {
        _httpClientFactory = httpClientFactory;
        _annonceExtensionService =  annonceExtensionService;
        _bloqueManager = bloqueManager;
        _annonceManager = annonceRepository;
        _currentUserService = currentUserService;
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
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(30);
                
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{_fastApiBaseUrl}/clustering/calculate", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var clusteringResult = JsonSerializer.Deserialize<ClusteringResponseDTO>(
                        responseContent,
                        new JsonSerializerOptions
                        {
                            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                            PropertyNameCaseInsensitive = true
                        });

                    if (clusteringResult != null && clusteringResult.Success)
                    {
                        await clusterRepository.DeleteByUserId(userId);
                        await clusterRepository.SaveClustersAsync(clusteringResult);
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

    public async Task<IEnumerable<AnnonceDTO>> GetRecommandations(int page, int pageSize)
    {
        int userId = await _currentUserService.GetUserIdOrThrow();
        List<int> blockedUserIds = await _bloqueManager.GetUserBlockedIds(userId);

        IEnumerable<Annonce> annonces = await _annonceManager.GetActiveAnnonces();
        IEnumerable<AnnoncePreferenceUtilisateur> clusters = await _annoncePreferenceManager.GetByUserId(userId);
        if (blockedUserIds.Any())
        {
            annonces = annonces.Where(a => !blockedUserIds.Contains(a.UtilisateurId));
        }
        if (!clusters.Any())
        {
            return _mapper.Map<IEnumerable<AnnonceDTO>>(
                annonces.OrderByDescending(a => a.DateAnnonce)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
            );
        }
        var annoncesAvecScore = annonces.Select(annonce => new
            {
                Annonce = annonce,
                Score = CalculerScore(annonce, clusters)
            })
            .OrderByDescending(x => x.Score)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => x.Annonce);
        IEnumerable<AnnonceDTO> annoncesDTO = await _annonceExtensionService.LikeAnnonces(_mapper.Map<IEnumerable<AnnonceDTO>>(annoncesAvecScore));
        return annoncesDTO;
    }
    private double CalculerScore(Annonce annonce, IEnumerable<AnnoncePreferenceUtilisateur> clusters)
{
    double scoreTotal = 0;
    double pondérationTotale = clusters.Sum(c => c.Ponderation);

    foreach (var cluster in clusters)
    {
        double scoreCluster = 0;
        const double POIDS_MARQUE = 0.30;           
        const double POIDS_COULEUR = 0.25;          
        const double POIDS_CATEGORIE = 0.15;        
        const double POIDS_SOUS_CATEGORIE = 0.10;   
        const double POIDS_PRIX = 0.10;             
        const double POIDS_TAILLE = 0.05;           
        const double POIDS_ETAT = 0.05;             

        if (annonce.Marque?.NomMarque != null && 
            annonce.Marque.NomMarque.Equals(cluster.NomMarque, StringComparison.OrdinalIgnoreCase))
        {
            scoreCluster += POIDS_MARQUE;
        }
        if (annonce.Couleurs?.Any() == true && !string.IsNullOrEmpty(cluster.CouleurDominante))
        {
            bool correspondanceCouleur = annonce.Couleurs.Any(c => 
                c.Couleur?.Nom != null && 
                c.Couleur.Nom.Equals(cluster.CouleurDominante, StringComparison.OrdinalIgnoreCase)
            );
            
            if (correspondanceCouleur)
            {
                scoreCluster += POIDS_COULEUR;
            }
        }

        if (annonce.Categorie?.LibelleCategorie != null && 
            annonce.Categorie.LibelleCategorie.Equals(cluster.Categorie, StringComparison.OrdinalIgnoreCase))
        {
            scoreCluster += POIDS_CATEGORIE;
        }

        
        if (annonce.SousCategorie?.LibelleSousCategorie != null && 
            annonce.SousCategorie.LibelleSousCategorie.Equals(cluster.SousCategorie, StringComparison.OrdinalIgnoreCase))
        {
            scoreCluster += POIDS_SOUS_CATEGORIE;
        }

        if (cluster.Prix > 0)
        {
            double prixAnnonce = (double)annonce.Prix;
            double ecartPrix = Math.Abs(prixAnnonce - cluster.Prix);
            double ecartRelatif = ecartPrix / cluster.Prix;
            double scorePrix = Math.Max(0, 1 - (ecartRelatif / 1.0));
            scoreCluster += POIDS_PRIX * scorePrix;
        }
        if (annonce.Taille?.Libelletaille != null && 
            annonce.Taille.Libelletaille.Equals(cluster.Taille, StringComparison.OrdinalIgnoreCase))
        {
            scoreCluster += POIDS_TAILLE;
        }
        if (annonce.Etat?.NomEtat != null && 
            annonce.Etat.NomEtat.Equals(cluster.EtatArticle, StringComparison.OrdinalIgnoreCase))
        {
            scoreCluster += POIDS_ETAT;
        }
        double facteurPonderation = cluster.Ponderation / pondérationTotale;
        scoreTotal += scoreCluster * facteurPonderation;
    }

    return scoreTotal;
}

private double CalculerScoreAvecBonus(Annonce annonce, IEnumerable<AnnoncePreferenceUtilisateur> clusters)
{
    double scoreTotal = 0;
    double pondérationTotale = clusters.Sum(c => c.Ponderation);

    foreach (var cluster in clusters)
    {
        double scoreCluster = CalculerScoreBase(annonce, cluster);
        
        bool correspondanceMarque = annonce.Marque?.NomMarque != null && 
            annonce.Marque.NomMarque.Equals(cluster.NomMarque, StringComparison.OrdinalIgnoreCase);
            
        bool correspondanceCouleur = annonce.Couleurs?.Any(c => 
            c.Couleur?.Nom != null && 
            c.Couleur.Nom.Equals(cluster.CouleurDominante, StringComparison.OrdinalIgnoreCase)
        ) == true;

        if (correspondanceMarque && correspondanceCouleur)
        {
            scoreCluster *= 1.5;
        }

        double facteurPonderation = cluster.Ponderation / pondérationTotale;
        scoreTotal += scoreCluster * facteurPonderation;
    }

    return scoreTotal;
}

private double CalculerScoreBase(Annonce annonce, AnnoncePreferenceUtilisateur cluster)
{
    double score = 0;
    
    if (annonce.Marque?.NomMarque != null && 
        annonce.Marque.NomMarque.Equals(cluster.NomMarque, StringComparison.OrdinalIgnoreCase))
        score += 0.30;

    if (annonce.Couleurs?.Any(c => 
        c.Couleur?.Nom != null && 
        c.Couleur.Nom.Equals(cluster.CouleurDominante, StringComparison.OrdinalIgnoreCase)) == true)
        score += 0.25;

    if (annonce.Categorie?.LibelleCategorie != null && 
        annonce.Categorie.LibelleCategorie.Equals(cluster.Categorie, StringComparison.OrdinalIgnoreCase))
        score += 0.15;

    if (annonce.SousCategorie?.LibelleSousCategorie != null && 
        annonce.SousCategorie.LibelleSousCategorie.Equals(cluster.SousCategorie, StringComparison.OrdinalIgnoreCase))
        score += 0.10;

    if (cluster.Prix > 0)
    {
        double ecartRelatif = Math.Abs((double)annonce.Prix - cluster.Prix) / cluster.Prix;
        score += 0.10 * Math.Max(0, 1 - ecartRelatif);
    }

    if (annonce.Taille?.Libelletaille != null && 
        annonce.Taille.Libelletaille.Equals(cluster.Taille, StringComparison.OrdinalIgnoreCase))
        score += 0.05;

    // État (5%)
    if (annonce.Etat?.NomEtat != null && 
        annonce.Etat.NomEtat.Equals(cluster.EtatArticle, StringComparison.OrdinalIgnoreCase))
        score += 0.05;

    return score;
}
}