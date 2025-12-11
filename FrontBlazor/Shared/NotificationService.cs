namespace FrontBlazor.Shared;

public class NotificationService
{
    public event Action<string, bool>? OnNotification; 
    
    public virtual void Show(string message, bool isSuccess = true)
    {
        OnNotification?.Invoke(message, isSuccess);
    }
}