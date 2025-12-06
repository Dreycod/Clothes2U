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

    // Méthode générique pour TOUTES les requêtes
    protected async Task<HttpResponseMessage> SendWithCredentialsAsync(
        HttpMethod method, 
        string url, 
        HttpContent? content = null)
    {
        var request = new HttpRequestMessage(method, url);
        
        if (content != null)
        {
            request.Content = content;
        }
        
        request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);
        return await _httpClient.SendAsync(request);
    }

    // Méthodes d'aide spécifiques
    protected Task<HttpResponseMessage> GetWithCredentialsAsync(string url)
        => SendWithCredentialsAsync(HttpMethod.Get, url);
    protected Task<HttpResponseMessage> PostWithCredentialsAsync(string url, HttpContent content)
        => SendWithCredentialsAsync(HttpMethod.Post, url, content);

    protected Task<HttpResponseMessage> PutWithCredentialsAsync(string url, HttpContent content)
        => SendWithCredentialsAsync(HttpMethod.Put, url, content);

    protected Task<HttpResponseMessage> DeleteWithCredentialsAsync(string url)
        => SendWithCredentialsAsync(HttpMethod.Delete, url);
}