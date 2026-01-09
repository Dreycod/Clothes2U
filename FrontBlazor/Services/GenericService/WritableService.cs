using Microsoft.AspNetCore.Components.WebAssembly.Http;
using Shared.DTO;
using Shared.Interfaces;
using System.Net.Http.Json;
using System.Numerics;
using System.Xml.Linq;
using FrontBlazor.Services.Interfaces.GenericIServices;

namespace FrontBlazor.Services.GenericService;

public class WritableService<T> : BaseGenericService, IWritableService<T>  where T : class, IEntity
{
    private string name = typeof(T).Name;
    public WritableService(HttpClient httpClient) : base(httpClient){

        if (name.EndsWith("PostDTO"))
        {
            name = name.Substring(0, name.Length - 7);
        }
        else if (name.EndsWith("DTO"))
        {
            name = name.Substring(0, name.Length - 3);
        }
    }
    
    
    public virtual async Task<T?> AddAsync(T entity)
    {
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
        var body = JsonContent.Create(updatedEntity);
        var response = await PutWithCredentialsAsync($"{_httpClient.BaseAddress}{name}/id/{updatedEntity.GetId()}", body);
        response.EnsureSuccessStatusCode();
    }

    public virtual async Task DeleteAsync(int id)
    {
        var response = await DeleteWithCredentialsAsync($"{_httpClient.BaseAddress}{name}/id/{id}");
        response.EnsureSuccessStatusCode();
    }
}