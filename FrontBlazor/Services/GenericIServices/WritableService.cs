using System.Net.Http.Json;
using FrontBlazor.Models;

namespace FrontBlazor.Services.GenericIServices;

public abstract class WritableService<T> : BaseGenericService, IWritableService<T>  where T : class, IEntity
{
    public WritableService(HttpClient httpClient) : base(httpClient){}
    
    
    public virtual async Task<T?> AddAsync(T entity)
    {
        var response = await _httpClient.PostAsJsonAsync($"api/{typeof(T).Name}", entity);
        if (!response.IsSuccessStatusCode)
            return null; 

        var createdProduct = await response.Content.ReadFromJsonAsync<T>();
        return createdProduct;
        
    }

    public virtual async Task UpdateAsync( T updatedEntity)
    {
        await _httpClient.PutAsJsonAsync($"api/{typeof(T).Name}/id/{updatedEntity.GetId()}", updatedEntity);
    }

    public virtual async Task DeleteAsync(int id)
    {
        await _httpClient.DeleteAsync($"api/{typeof(T).Name}/id/{id}");
    }
}