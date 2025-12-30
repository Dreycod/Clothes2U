using System.Net.Http.Json;
using System.Xml.Linq;

namespace FrontBlazor.Services.GenericIServices;

public class ListableService<T >: BaseGenericService, IListableService<T> where T : class
{
    
    public ListableService(HttpClient httpClient) : base(httpClient){}
    
    public virtual async Task<List<T>?> GetAllAsync()
    {
        string name = typeof(T).Name;
        if (name.EndsWith("DTO"))
        {
            name = name.Substring(0, name.Length - 3);
        }
        return await _httpClient.GetFromJsonAsync<List<T>>($"{name}");

    }
    public virtual async Task<List<T>?> GetAllWithDetailsAsync()
    {
        string name = typeof(T).Name;
        if (name.EndsWith("DTO"))
        {
            name = name.Substring(0, name.Length - 3);
        }
        return await _httpClient.GetFromJsonAsync<List<T>>($"{name}/details");
    }
}