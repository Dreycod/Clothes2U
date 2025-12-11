using System.Collections.ObjectModel;
using System.Text.Json;
using FrontBlazor.Models.Notification;
using FrontBlazor.Converters;
using FrontBlazor.Services.Interfaces;

namespace FrontBlazor.Services;

public class NotificationWebService : BaseGenericService, INotificationService 
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        Converters = { new NotificationJsonConverter() }
    };
    public NotificationWebService(HttpClient httpClient) : base(httpClient) { }
    public async Task<ObservableCollection<Notification>> GetAllAsync(int utilisateurId)
    {
        try
        {
            var response = await GetWithCredentialsAsync($"Notification/user/{utilisateurId}");
            response.EnsureSuccessStatusCode();
            
            var jsonString = await response.Content.ReadAsStringAsync();
            var notificationsList = JsonSerializer.Deserialize<List<Notification>>(jsonString, JsonOptions);
            
            return new ObservableCollection<Notification>(notificationsList ?? new List<Notification>());
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            return new ObservableCollection<Notification>();
        }
    }
    public async Task MarkAsRead()
    {
        await PutWithCredentialsAsync("Notification/markAsRead,");
    }
    public async Task DeleteNotification(int id)
    {
        await DeleteWithCredentialsAsync($"Notification/{id}");
    }
}