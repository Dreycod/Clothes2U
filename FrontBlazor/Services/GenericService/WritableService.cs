using Shared.DTO;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net.Http.Json;
using System.Numerics;
using Shared.Interfaces;

namespace FrontBlazor.Services.GenericIServices;

public class WritableService<T> : BaseGenericService, IWritableService<T>  where T : class, IEntity
{
    public WritableService(HttpClient httpClient) : base(httpClient){}
    
    
    public virtual async Task<T?> AddAsync(T entity)
    {
        string name = typeof(T).Name;

        if (name.EndsWith("DTO"))
        {
            name = name.Substring(0, name.Length - 3);
        }

        var request = new HttpRequestMessage(HttpMethod.Post, $"{_httpClient.BaseAddress}{name}")
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
        string name = typeof(T).Name;
        if (name.EndsWith("DTO"))
        {
            name = name.Substring(0, name.Length - 3);
        }

        var body = JsonContent.Create(updatedEntity);
        var response = await PutWithCredentialsAsync($"{_httpClient.BaseAddress}{name}/id/{updatedEntity.GetId()}", body);
        response.EnsureSuccessStatusCode();
    }

    public virtual async Task DeleteAsync(int id)
    {
        string name = typeof(T).Name;
        if (name.EndsWith("DTO"))
        {
            name = name.Substring(0, name.Length - 3);
        }
        var response = await DeleteWithCredentialsAsync($"{_httpClient.BaseAddress}{name}/id/{id}");
        response.EnsureSuccessStatusCode();
    }
}