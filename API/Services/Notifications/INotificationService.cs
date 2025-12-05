namespace API.Services.Notifications;

public interface INotificationService
{
    void Subscribe<T>() where T : INotificationObserver;
    Task NotifyAsync(INotificationEvent notificationEvent);
}

public class NotificationService : INotificationService
{
    private readonly List<Type> _observerTypes = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(IServiceProvider serviceProvider, ILogger<NotificationService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public void Subscribe<T>() where T : INotificationObserver
    {
        var observerType = typeof(T);
        if (!_observerTypes.Contains(observerType))
        {
            _observerTypes.Add(observerType);
            _logger.LogInformation("Observer {ObserverType} enregistré", observerType.Name);
        }
    }

    public async Task NotifyAsync(INotificationEvent notificationEvent)
    {
        using var scope = _serviceProvider.CreateScope();
        var notificationType = notificationEvent.GetNotificationType();

        var tasks = new List<Task>();

        foreach (var observerType in _observerTypes)
        {
            var observer = (INotificationObserver)scope.ServiceProvider.GetRequiredService(observerType);
            
            if (observer.GetSupportedTypes().Contains(notificationType))
            {
                tasks.Add(ExecuteObserverSafely(observer, notificationEvent));
            }
        }

        if (!tasks.Any())
        {
            _logger.LogWarning("Aucun observer trouvé pour le type de notification {NotificationType}", notificationType);
            return;
        }

        _logger.LogInformation(
            "Notification de type {NotificationType} envoyée à {ObserverCount} observers",
            notificationType, tasks.Count);

        await Task.WhenAll(tasks);
    }

    private async Task ExecuteObserverSafely(INotificationObserver observer, INotificationEvent notificationEvent)
    {
        try
        {
            await observer.HandleNotificationAsync(notificationEvent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Erreur lors de l'exécution de l'observer {ObserverType} pour l'événement {EventType}",
                observer.GetType().Name, notificationEvent.GetType().Name);
        }
    }
}