using System.Net.Http.Json;
using FrontBlazor.Services.Interfaces.GenericIServices;

namespace FrontBlazor.Services.GenericService;

public class ReadableService<T> : BaseGenericService, IReadableService<T> where T : class
{
    public ReadableService(HttpClient httpClient) : base(httpClient){}
    
    public virtual async Task<T?> GetByIdAsync(int id)
    {
        return await _httpClient.GetFromJsonAsync<T?>($"{typeof(T).Name}/{id}");
    }
}