using Shared.DTO;
using FrontBlazor.Services.GenericIServices;
using System.Globalization;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.WebUtilities;
using Shared.DTO.Annonce;

namespace FrontBlazor.Services;

public class AnnonceWebService : BaseGenericService, IAnnonceService
{
    public AnnonceWebService(HttpClient httpClient) : base(httpClient) { }

    public async Task<List<AnnonceDTO>> GetActiveAnnonces()
    {
        var response = await GetWithCredentialsAsync("Annonce/GetActiveAnnonces");
        response.EnsureSuccessStatusCode();

        var annonces = await response.Content.ReadFromJsonAsync<List<AnnonceDTO>>();

        return annonces ?? new List<AnnonceDTO>();
    }
    public async Task<AnnonceDetailDTO> GetAnnonceDetailById(int Id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<AnnonceDetailDTO>($"Annonce/id/{Id}");
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

    public Task<AnnonceDTO> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<AnnonceDTO>?> GetAnnonceByFilter(FilterDTO filterDto, int page = 1, int pageSize = 20)
    {
        var queryParams = new List<KeyValuePair<string, string?>>();

        if (!string.IsNullOrWhiteSpace(filterDto.MotCle))
            queryParams.Add(new("MotCle", filterDto.MotCle));
        AddListToQuery(queryParams, "Marques", filterDto.Marques);
        AddListToQuery(queryParams, "Categories", filterDto.Categories);
        AddListToQuery(queryParams, "SousCategories", filterDto.SousCategories);
        AddListToQuery(queryParams, "Genre", filterDto.Genre);
        AddListToQuery(queryParams, "Etats", filterDto.Etats);
        AddListToQuery(queryParams, "Tailles", filterDto.Tailles);

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

    public async Task<List<AnnonceDTO>> GetByFavorisUtilisateur()
    {
        var response = await GetWithCredentialsAsync("Annonce/ByFavorisUtilisateur");
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

    public async Task CreateAnnonce(CreateAnnonceDTO annonce)
    {
        var body = JsonContent.Create(annonce);
        
        var response = await PostWithCredentialsAsync("Annonce", body);
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

    public async Task<List<AnnonceDTO>> GetRecommendedAnnonces(int page, int pageSize )
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

    public async Task VendreAnnonce(int AnnonceId)
    {
        var response = await PutWithCredentialsAsync($"Annonce/Vendu/{AnnonceId}", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateAnnonce(int id, PutAnnonceDTO annonce)
    {
        var body = JsonContent.Create(annonce);
        var response = await PutWithCredentialsAsync($"Annonce/id/{id}", body);
        response.EnsureSuccessStatusCode();
    }
}