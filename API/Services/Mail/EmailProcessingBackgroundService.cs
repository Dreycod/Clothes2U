using Microsoft.Extensions.Hosting;

namespace API.Services
{
    public class EmailProcessingBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EmailProcessingBackgroundService> _logger;
        private readonly IConfiguration _config;

        public EmailProcessingBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<EmailProcessingBackgroundService> logger,
            IConfiguration config)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
            _config = config;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Service de traitement des emails démarré");

            // Délai entre chaque vérification (en minutes)
            var checkIntervalMinutes = _config.GetValue<int>("Email:CheckIntervalMinutes", 2);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Vérification des nouveaux emails...");

                    using var scope = _scopeFactory.CreateScope();
                    var emailReceiver = scope.ServiceProvider.GetRequiredService<IEmailReceiverService>();

                    await emailReceiver.ProcessIncomingEmailsAsync();

                    _logger.LogInformation("Vérification terminée. Prochaine vérification dans {Minutes} minutes", checkIntervalMinutes);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Erreur lors du traitement des emails");
                }

                // Attendre avant la prochaine vérification
                await Task.Delay(TimeSpan.FromMinutes(checkIntervalMinutes), stoppingToken);
            }

            _logger.LogInformation("Service de traitement des emails arrêté");
        }
    }
}