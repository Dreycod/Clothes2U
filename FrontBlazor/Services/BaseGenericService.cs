namespace FrontBlazor.Services;

public abstract class BaseGenericService
{
    protected readonly HttpClient  _httpClient;

    public BaseGenericService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }
}