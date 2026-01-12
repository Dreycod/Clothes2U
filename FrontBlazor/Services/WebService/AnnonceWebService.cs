using FrontBlazor.Exceptions;
using FrontBlazor.Services.GenericService;
using FrontBlazor.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;
using Shared.DTO;
using Shared.DTO.Annonce;
using System.Globalization;
using System.Net.Http.Json;
using System.Runtime.Serialization;

namespace FrontBlazor.Services;

public class AnnonceWebService : BaseGenericService, IAnnonceService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<AnnonceWebService> _logger;
    public AnnonceWebService(HttpClient httpClient, ILogger<AnnonceWebService> logger) : base(httpClient)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<AnnonceDetailDTO?> GetAnnonceDetailById(int Id)
    {
        try
        {
            var response = await GetWithCredentialsAsync($"Annonce/id/{Id}");
            response.EnsureSuccessStatusCode();
            var annonce = await response.Content.ReadFromJsonAsync<AnnonceDetailDTO>();
            return annonce ?? null;
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Error: {ex.Message}");
            return null;
        }
    }
    public async Task<List<AnnonceDTO?>?> GetAnnoncesByUserIdAsync(int userId)
    {
        try
        {
            var response = await GetWithCredentialsAsync($"Annonce/ByUtilisateurId/{userId}");
            response.EnsureSuccessStatusCode();
            var annonces = await response.Content.ReadFromJsonAsync<List<AnnonceDTO>>();
            return annonces ?? new List<AnnonceDTO>();
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Error: {ex.Message}");
            return null;
        }
    }

    public async Task<List<AnnonceDTO>?> GetAnnonceByFilter(FilterDTO filterDto, int page = 1, int pageSize = 20)
    {
        var queryParams = new List<KeyValuePair<string, string?>>();

        if (!string.IsNullOrWhiteSpace(filterDto.MotCle))
            queryParams.Add(new("MotCle", filterDto.MotCle));

        AddListToQuery(queryParams, "Marques", filterDto.Marques);

        if (!string.IsNullOrWhiteSpace(filterDto.Categories))
            queryParams.Add(new("Categories", filterDto.Categories));

        if (!string.IsNullOrWhiteSpace(filterDto.SousCategories))
            queryParams.Add(new("SousCategories", filterDto.SousCategories));

        AddListToQuery(queryParams, "Genre", filterDto.Genre);
        AddListToQuery(queryParams, "Etats", filterDto.Etats);

        if (!string.IsNullOrWhiteSpace(filterDto.Tailles))
            queryParams.Add(new("Tailles", filterDto.Tailles));

        if (filterDto.PrixMin.HasValue)
            queryParams.Add(new("PrixMin", filterDto.PrixMin.Value.ToString(CultureInfo.InvariantCulture)));

        if (filterDto.PrixMax.HasValue)
            queryParams.Add(new("PrixMax", filterDto.PrixMax.Value.ToString(CultureInfo.InvariantCulture)));

        if (filterDto.SortBy.HasValue)
            queryParams.Add(new("SortBy", ((int)filterDto.SortBy.Value).ToString()));

        queryParams.Add(new("SortOrder", ((int)filterDto.SortOrder).ToString()));

        queryParams.Add(new("page", page.ToString()));
        queryParams.Add(new("pageSize", pageSize.ToString()));

        var url = QueryHelpers.AddQueryString("Annonce/productByFilter", queryParams);
        Console.WriteLine(filterDto.Etats);
        var response = await GetWithCredentialsAsync(url);
        response.EnsureSuccessStatusCode();

        var annonces = await response.Content.ReadFromJsonAsync<List<AnnonceDTO>>();
        return annonces ?? new List<AnnonceDTO>();
    }

    public async Task<List<AnnonceDTO>> GetByFavorisUtilisateur(int page = 1, int pageSize = 8)
    {
        var response = await GetWithCredentialsAsync($"Annonce/ByFavorisUtilisateur?page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        var annonces = await response.Content.ReadFromJsonAsync<List<AnnonceDTO>>();
        return annonces ?? new List<AnnonceDTO>();
    }

    private void AddListToQuery(List<KeyValuePair<string, string?>> qp, string key, List<string>? values)
    {
        if (values == null) return;

        foreach (var v in values)
            qp.Add(new(key, v));
    }

    public async Task<AnnonceDTO?> CreateAnnonce(CreateAnnonceDTO createAnnonceDto)
    {
        try
        {
            _logger.LogInformation("Tentative de cr�ation d'annonce: {Titre}", createAnnonceDto.Titre);
            var body = JsonContent.Create(createAnnonceDto);
            var response = await PostWithCredentialsAsync("Annonce", body);

            // ? G�RER SP�CIFIQUEMENT LE BADREQUEST (400)
            if (response.StatusCode == System.Net.HttpStatusCode.BadRequest)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("Cr�ation annonce refus�e (BadRequest): {Error}", errorContent);

                // ? V�rifier si c'est un mot interdit
                if (errorContent.Contains("Mot Interdit", StringComparison.OrdinalIgnoreCase))
                {
                    throw new MotInterditException("Votre annonce contient un mot interdit. Veuillez modifier le titre ou la description.");
                }

                // Autre type de BadRequest
                throw new BadRequestException(errorContent, 400);
            }

            // ? V�rifier le succ�s
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                _logger.LogError("Erreur cr�ation annonce: {StatusCode} - {Error}", response.StatusCode, error);
                throw new HttpRequestException($"Erreur serveur ({response.StatusCode}): {error}");
            }

            // ? Succ�s - Retourner l'annonce cr��e
            var createdAnnonce = await response.Content.ReadFromJsonAsync<AnnonceDTO>();
            _logger.LogInformation("Annonce cr��e avec succ�s: ID={AnnonceId}", createdAnnonce?.AnnonceId);

            return createdAnnonce;
        }
        catch (MotInterditException)
        {
            // ? Relancer l'exception pour qu'elle soit captur�e par le ViewModel
            throw;
        }
        catch (BadRequestException)
        {
            // ? Relancer l'exception pour qu'elle soit captur�e par le ViewModel
            throw;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Erreur r�seau lors de la cr�ation d'annonce");
            throw new Exception("Erreur de connexion au serveur", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur inattendue lors de la cr�ation d'annonce");
            throw;
        }
    }

    public async Task ModificationAnnonce(AnnonceDetailDTO annonceDTO)
    {
        var response = await PutWithCredentialsAsync(
            $"Annonce/id/{annonceDTO.AnnonceId}",
            JsonContent.Create(annonceDTO)
        );

        response.EnsureSuccessStatusCode();
    }

    public async Task<List<AnnonceDTO>?> GetSimilarAnnonces(int annonceId, int page, int pageSize)
    {
        var queryParams = new List<KeyValuePair<string, string?>>();

        queryParams.Add(new("annonceId", annonceId.ToString()));
        queryParams.Add(new("page", page.ToString()));
        queryParams.Add(new("pageSize", pageSize.ToString()));

        var url = QueryHelpers.AddQueryString("Annonce/similarAnnonces", queryParams);

        var response = await GetWithCredentialsAsync(url);
        response.EnsureSuccessStatusCode();

        var annonces = await response.Content.ReadFromJsonAsync<List<AnnonceDTO>>();
        return annonces ?? new List<AnnonceDTO>();
    }

    public async Task<List<AnnonceDTO>> GetRecommendedAnnonces(int page, int pageSize)
    {
        var queryParams = new List<KeyValuePair<string, string?>>();
        queryParams.Add(new("page", page.ToString()));
        queryParams.Add(new("pageSize", pageSize.ToString()));
        var url = QueryHelpers.AddQueryString("Annonce/Recommandations", queryParams);
        var response = await GetWithCredentialsAsync(url);
        response.EnsureSuccessStatusCode();
        var annonces = await response.Content.ReadFromJsonAsync<List<AnnonceDTO>>();
        return annonces ?? new List<AnnonceDTO>();
    }
    public async Task<IEnumerable<AnnonceDTO>> GetAnnoncesByPhotoIDs(IEnumerable<int> photoIDs)
    {
        if (photoIDs == null || !photoIDs.Any())
            return Enumerable.Empty<AnnonceDTO>();

        var query = string.Join("&", photoIDs.Select(id => $"PhotoIDs={id}"));
        var response = await GetWithCredentialsAsync($"Annonce/GetAnnoncesByPhotoIDs?{query}");
        response.EnsureSuccessStatusCode();

        var annonces = await response.Content.ReadFromJsonAsync<List<AnnonceDTO>>();
        return annonces ?? Enumerable.Empty<AnnonceDTO>();
    }

    public async Task VendreAnnonce(int AnnonceId)
    {
        var response = await PutWithCredentialsAsync($"Annonce/Vendu/{AnnonceId}", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteAnnonce(int annonceId)
    {
        var response = await DeleteWithCredentialsAsync($"Annonce/id/{annonceId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task<List<AnnonceDTO>?> GetAnnoncesPaginationByUserIdAsync(int id, int page = 1, int pageSize = 8)
    {
        var response = await GetWithCredentialsAsync($"Annonce/ByUtilisateurIdPagination/{id}?page={page}&pageSize={pageSize}");
        response.EnsureSuccessStatusCode();
        var annonces = await response.Content.ReadFromJsonAsync<List<AnnonceDTO>>();
        return annonces ?? new List<AnnonceDTO>();
    }

    public async Task UpdateAnnonce(int id, PutAnnonceDTO annonce)
    {
        var body = JsonContent.Create(annonce);
        var response = await PutWithCredentialsAsync($"Annonce/id/{id}", body);
        response.EnsureSuccessStatusCode();
        return;
    }

    public async Task ReprendreAnnonce(int annonceId)
    {
        var response = await PatchWithCredentialsAsync($"Annonce/ReprendreAnnonce/{annonceId}");
        response.EnsureSuccessStatusCode();
    }

    public async Task PauseAnnonce(int annonceId)
    {
        var response = await PatchWithCredentialsAsync($"Annonce/PauseAnnonce/{annonceId}");
        response.EnsureSuccessStatusCode();
    }
}