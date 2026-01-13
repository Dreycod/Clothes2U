using API.Services.Interfaces;

namespace API.Services.BackgroundServices;

public class MasterDailyBackgroundService: BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConfiguration _configuration;
    private readonly IServiceScopeFactory _scopeFactory;


    public MasterDailyBackgroundService(
        IServiceProvider serviceProvider,
        IServiceScopeFactory scopeFactory,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _configuration = configuration;
        _scopeFactory = scopeFactory;
    }
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var isDevelopment = _configuration.GetValue<bool>("IsDevelopment");
        
        while (!stoppingToken.IsCancellationRequested)
        {
            
            TimeSpan delay;
            
            if (isDevelopment)
            {
                delay = TimeSpan.FromMinutes(1);
            }
            else
            {
                var now = DateTime.Now;
                var nextRun = DateTime.Today.AddDays(1).AddHours(2);
                
                if (now > nextRun)
                    nextRun = nextRun.AddDays(1);
                
                delay = nextRun - now;
            }
            
            await Task.Delay(delay, stoppingToken);
            await RunAllProcessors(stoppingToken);
        }
    }
    private async Task RunAllProcessors(CancellationToken cancellationToken = default)
    {
        using var scope = _serviceProvider.CreateScope();
        
        try
        {
            var suspendedUserDailyCheckService =
                scope.ServiceProvider.GetRequiredService<ISuspendedUserDailyCheckService>();

            await suspendedUserDailyCheckService.CheckSuspensions();
            
            
            var oldPendingTicketService =
                scope.ServiceProvider.GetRequiredService<ITicketDailyClosingService>();

            await oldPendingTicketService.CloseOldTickets();
            
            var deleteOldNotificationService =
                scope.ServiceProvider.GetRequiredService<IDailyDeleteReadNotificationService>();

            await deleteOldNotificationService.DeleteReadOldNotifications();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex +  "Erreur lors du traitement");
        }
    }
}