using System.Collections.ObjectModel;
using System.Text;
using System.Text.Json;
using Shared.DTO.Notification;
using Shared.DTO.Moderation;
using FrontBlazor.Services.GenericService;
using FrontBlazor.Services.Interfaces;

namespace FrontBlazor.Services;

public class NotificationWebService : BaseGenericService, INotificationService 
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    public NotificationWebService(HttpClient httpClient) : base(httpClient) { }
    public async Task<ObservableCollection<NotificationDTO>> GetAllAsync()
    {
        try
        {
            var response = await GetWithCredentialsAsync("Notification/user");
            response.EnsureSuccessStatusCode();
            
            var jsonString = await response.Content.ReadAsStringAsync();
            var notificationsList = JsonSerializer.Deserialize<List<NotificationDTO>>(jsonString, JsonOptions);
            return new ObservableCollection<NotificationDTO>(notificationsList ?? new List<NotificationDTO>());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            return new ObservableCollection<NotificationDTO>();
        }
    }
    public async Task DeleteNotification(int id)
    {
        await DeleteWithCredentialsAsync($"Notification/{id}");
    }

    public async Task PostCommercialNotification(NotificationCommercialCreateDTO notification)
    {
        var response = await PostWithCredentialsAsync(
            "Notification/commercial",
            new StringContent(JsonSerializer.Serialize(notification), Encoding.UTF8, "application/json")
        );
        
        if (response.IsSuccessStatusCode)
            return;

        var error = await response.Content.ReadAsStringAsync();

        if (string.IsNullOrWhiteSpace(error))
            error = $"Erreur HTTP {(int)response.StatusCode}";

        throw new Exception(error);
    }

}

