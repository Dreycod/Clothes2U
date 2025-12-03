using System.Net.Http.Json;
using Microsoft.AspNetCore.Components.WebAssembly.Http;


namespace FrontBlazor.Services;

public abstract class BaseGenericService
{
    protected readonly HttpClient _httpClient;

    public BaseGenericService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    protected async Task<HttpResponseMessage> SendWithCredentialsAsync(HttpRequestMessage request)
    {
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        return await _httpClient.SendAsync(request);
    }
}