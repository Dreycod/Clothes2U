using FrontBlazor.Models;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net.Http.Json;
using System.Numerics;

namespace FrontBlazor.Services.GenericIServices;

public class WritableService<T> : BaseGenericService, IWritableService<T>  where T : class, IEntity
{
    public WritableService(HttpClient httpClient) : base(httpClient){}
    
    
    public virtual async Task<T?> AddAsync(T entity)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}{typeof(T).Name}")
        {
            Content = JsonContent.Create(entity)
        };
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

        var response = await _httpClient.SendAsync(request);

        if (!response.IsSuccessStatusCode)
            return null;

        var createdProduct = await response.Content.ReadFromJsonAsync<T>();
        return createdProduct;        
    }

    public virtual async Task UpdateAsync( T updatedEntity)
    {
        var body = JsonContent.Create(updatedEntity);
        var response = await PutWithCredentialsAsync($"{_httpClient.BaseAddress}{typeof(T).Name}/id/{updatedEntity.GetId()}", body);
        response.EnsureSuccessStatusCode();
    }

    public virtual async Task DeleteAsync(int id)
    {
        var response = await DeleteWithCredentialsAsync($"{_httpClient.BaseAddress}{typeof(T).Name}/id/{id}");
        response.EnsureSuccessStatusCode();
    }
}