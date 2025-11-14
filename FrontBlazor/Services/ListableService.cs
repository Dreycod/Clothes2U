namespace FrontBlazor.Services;

public abstract class ListableService<T >: BaseGenericService, IListableService<T> where T : class
{
    
    public ListableService(HttpClient httpClient) : base(httpClient){}
    
    public virtual async Task<List<T>?> GetAllAsync()
    {
        return await _httpClient.GetFromJsonAsync<List<T>>($"{typeof(T).Name}");
    }
}