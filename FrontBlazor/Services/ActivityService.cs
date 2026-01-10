using FrontBlazor.Services.Interfaces;

namespace FrontBlazor.Services;

public class ActivityService
{
    public int NotificationCount { get; private set; }
    public int MessageCount { get; private set; }
    
    public event Action? OnCountersChanged;
    
    public async Task UpdateCounters(IUtilisateurService utilisateurService)
    {
        var data = await utilisateurService.GetActivity();
        NotificationCount = data.NotificationsCount;
        MessageCount = data.MessagesCount;
        OnCountersChanged?.Invoke();
    }
}