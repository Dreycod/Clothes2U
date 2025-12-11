using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;
using System.Globalization;
using System.Net.Http.Json;
using System.Runtime.Serialization;
using Microsoft.AspNetCore.WebUtilities;

namespace FrontBlazor.Services;

public class AnnonceWebService : WritableService<Annonce>, IAnnonceService<Annonce>
{
    public AnnonceWebService(HttpClient httpClient) : base(httpClient) { }

    public async Task<List<Annonce>> GetActiveAnnonces()
    {
        var response = await GetWithCredentialsAsync("Annonce/GetActiveAnnonces");
        response.EnsureSuccessStatusCode();

        var annonces = await response.Content.ReadFromJsonAsync<List<Annonce>>();

        return annonces ?? new List<Annonce>();
    }

    public async Task<List<Annonce>?> GetAnnoncesByCategorieId(int Id)
    {
        return await _httpClient.GetFromJsonAsync<List<Annonce>>(
            $"Annonce/ByCategorieId/{Id}"
        );
    }
    public async Task<List<Annonce?>?> GetAnnoncesBySousCategoryId(int Id)
    {
        return await _httpClient.GetFromJsonAsync<List<Annonce>>(
            $"Annonce/BySousCategorieId/{Id}"
        );
    }
    public async Task<Annonce> GetAnnonceDetailById(int Id)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<Annonce>($"Annonce/id/{Id}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Error: {ex.Message}");
            return null;
        }
    }
    public async Task<List<Annonce?>?> GetAnnoncesByUserIdAsync(int userId)
    {
        try
        {
            return await _httpClient.GetFromJsonAsync<List<Annonce>>($"Annonce/ByUtilisateurId/{userId}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"HTTP Error: {ex.Message}");
            return null;
        }
    }

    public Task<Annonce> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<List<Annonce>?> GetAnnonceByFilter(FilterDTO filterDto, int page = 1, int pageSize = 20)
    {
        var queryParams = new List<KeyValuePair<string, string?>>();

        if (!string.IsNullOrWhiteSpace(filterDto.MotCle))
            queryParams.Add(new("MotCle", filterDto.MotCle));

        // Listes : Marques, Categories, etc.
        AddListToQuery(queryParams, "Marques", filterDto.Marques);
        AddListToQuery(queryParams, "Categories", filterDto.Categories);
        AddListToQuery(queryParams, "SousCategories", filterDto.SousCategories);
        AddListToQuery(queryParams, "Tailles", filterDto.Tailles);
        AddListToQuery(queryParams, "Etats", filterDto.Etats);

        if (filterDto.PrixMin.HasValue)
            queryParams.Add(new("PrixMin", filterDto.PrixMin.Value.ToString(CultureInfo.InvariantCulture)));

        if (filterDto.PrixMax.HasValue)
            queryParams.Add(new("PrixMax", filterDto.PrixMax.Value.ToString(CultureInfo.InvariantCulture)));

        // Enums 
        if (filterDto.SortBy.HasValue)
            queryParams.Add(new("SortBy", ((int)filterDto.SortBy.Value).ToString()));

        // SortOrder n'est pas nullable (donc on send tj)
        queryParams.Add(new("SortOrder", ((int)filterDto.SortOrder).ToString()));

        // Pagination 
        queryParams.Add(new("page", page.ToString()));
        queryParams.Add(new("pageSize", pageSize.ToString()));

        //Construit l'URL finale : Annonce/productByFilter?MotCle=...&Marques=...&...
        var url = QueryHelpers.AddQueryString("Annonce/productByFilter", queryParams);

        var response = await GetWithCredentialsAsync(url);
        response.EnsureSuccessStatusCode();

        var annonces = await response.Content.ReadFromJsonAsync<List<Annonce>>();
        return annonces ?? new List<Annonce>();
    }

    public async Task<List<Annonce>> GetByFavorisUtilisateur()
    {
        var response = await GetWithCredentialsAsync("Annonce/ByFavorisUtilisateur");
        response.EnsureSuccessStatusCode();

        var annonces = await response.Content.ReadFromJsonAsync<List<Annonce>>();

        return annonces ?? new List<Annonce>();
    }

    private void AddListToQuery(List<KeyValuePair<string, string?>> qp, string key, List<string>? values)
    {
        if (values == null) return;

        foreach (var v in values)
            qp.Add(new(key, v));
    }
}