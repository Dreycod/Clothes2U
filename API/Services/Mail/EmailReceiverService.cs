using MailKit;
using MailKit.Net.Imap;
using MimeKit;
using API.Models.EntityFramework;
using API.Models.Repository;
using API.Models.Repository.Interfaces;
using MailKit.Search;
using Shared.Enums;

namespace API.Services
{
    public interface IEmailReceiverService
    {
        Task ProcessIncomingEmailsAsync();
    }

    public class EmailReceiverService : IEmailReceiverService
    {
        private readonly IConfiguration _config;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<EmailReceiverService> _logger;

        public EmailReceiverService(
            IConfiguration config,
            IServiceScopeFactory scopeFactory,
            ILogger<EmailReceiverService> logger)
        {
            _config = config;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        public async Task ProcessIncomingEmailsAsync()
        {
            try
            {
                var imapServer = _config["Email:ImapServer"];
                var imapPortStr = _config["Email:ImapPort"];
                var username = _config["Email:Username"];
                var password = _config["Email:Password"];

                if (string.IsNullOrEmpty(imapServer) || 
                    string.IsNullOrEmpty(imapPortStr) || 
                    string.IsNullOrEmpty(username) || 
                    string.IsNullOrEmpty(password))
                {
                    _logger.LogWarning("Configuration IMAP incomplète. Vérifiez votre appsettings.json");
                    _logger.LogWarning($"ImapServer: {imapServer}, ImapPort: {imapPortStr}, Username: {username}");
                    return;
                }

                if (!int.TryParse(imapPortStr, out int imapPort))
                {
                    _logger.LogError($"Le port IMAP '{imapPortStr}' n'est pas un nombre valide");
                    return;
                }

                using var client = new ImapClient();

                _logger.LogInformation($"Tentative de connexion à {imapServer}:{imapPort}");

                // Connexion au serveur IMAP
                await client.ConnectAsync(
                    imapServer,
                    imapPort,
                    MailKit.Security.SecureSocketOptions.SslOnConnect
                );

                await client.AuthenticateAsync(username, password);

                var inbox = client.Inbox;
                await inbox.OpenAsync(FolderAccess.ReadWrite);

                // Récupérer uniquement les emails non lus
                var unreadMessages = await inbox.SearchAsync(SearchQuery.NotSeen);

                _logger.LogInformation($"Nombre d'emails non lus : {unreadMessages.Count}");

                foreach (var uid in unreadMessages)
                {
                    var message = await inbox.GetMessageAsync(uid);
                    await ProcessEmailAsync(message);

                    // Marquer comme lu après traitement
                    await inbox.AddFlagsAsync(uid, MessageFlags.Seen, true);
                }

                await client.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la récupération des emails");
                throw;
            }
        }

        private async Task ProcessEmailAsync(MimeMessage email)
        {
            using var scope = _scopeFactory.CreateScope();
            var ticketManager = scope.ServiceProvider.GetRequiredService<ITicketRepository>();
            var messageManager = scope.ServiceProvider.GetRequiredService<IDataRepository<TicketMessage, int>>();
            var utilisateurManager = scope.ServiceProvider.GetRequiredService<IUtilisateurRepository>();

            try
            {
                _logger.LogInformation($"Traitement de l'email : {email.Subject} de {email.From}");

                // Extraire l'ID du ticket depuis l'adresse To
                int? ticketId = ExtractTicketIdFromEmail(email);

                if (!ticketId.HasValue)
                {
                    _logger.LogWarning($"Impossible d'extraire l'ID du ticket pour l'email : {email.Subject}");
                    _logger.LogWarning($"To: {string.Join(", ", email.To)}");
                    return;
                }

                _logger.LogInformation($"Traitement de la réponse pour le ticket #{ticketId}");

                // Vérifier que le ticket existe
                var ticket = await ticketManager.GetByIdAsync(ticketId.Value);
                if (ticket == null)
                {
                    _logger.LogWarning($"Ticket #{ticketId} introuvable");
                    return;
                }

                // Vérifier que le ticket n'est pas fermé
                if (ticket.Status == (int)StatusTicketEnum.CLOSED)
                {
                    _logger.LogWarning($"Tentative de réponse à un ticket fermé #{ticketId}");
                    return;
                }

                // Récupérer l'utilisateur du ticket
                var utilisateur = await utilisateurManager.GetByIdAsync(ticket.UtilisateurId);
                if (utilisateur == null)
                {
                    _logger.LogWarning($"Utilisateur #{ticket.UtilisateurId} introuvable pour le ticket #{ticketId}");
                    return;
                }

                // Vérifier que l'expéditeur correspond bien à l'utilisateur du ticket
                var senderEmail = (email.From[0] as MailboxAddress)?.Address;
                if (string.IsNullOrEmpty(senderEmail))
                {
                    _logger.LogWarning($"Impossible d'extraire l'email de l'expéditeur");
                    return;
                }

                if (!senderEmail.Equals(utilisateur.Email, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning($"Email expéditeur ({senderEmail}) ne correspond pas à l'utilisateur du ticket ({utilisateur.Email})");
                    return;
                }

                // Extraire le contenu de l'email
                var emailContent = ExtractEmailContent(email);
                if (string.IsNullOrWhiteSpace(emailContent))
                {
                    _logger.LogWarning($"Contenu de l'email vide pour le ticket #{ticketId}");
                    return;
                }

                _logger.LogInformation($"Contenu extrait : {emailContent.Substring(0, Math.Min(100, emailContent.Length))}...");

                // Créer le message dans la base de données
                var ticketMessage = new TicketMessage
                {
                    TicketId = ticketId.Value,
                    UtilisateurId = utilisateur.UtilisateurId,
                    Content = emailContent,
                    DateEnvoi = DateTime.UtcNow
                };

                await messageManager.AddAsync(ticketMessage);

                // Mettre à jour le statut du ticket à PENDING (en attente de réponse du support)
                ticket.Status = (int)StatusTicketEnum.OPEN;
                await ticketManager.UpdateAsync(ticket);

                _logger.LogInformation($"✅ Message ajouté avec succès au ticket #{ticketId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erreur lors du traitement de l'email : {email.Subject}");
            }
        }

        private int? ExtractTicketIdFromEmail(MimeMessage email)
        {
            if (!string.IsNullOrEmpty(email.Subject))
            {
                _logger.LogInformation($"Analyse du sujet: {email.Subject}");
        
                var match = System.Text.RegularExpressions.Regex.Match(
                    email.Subject,
                    @"\[Ticket\s*#(\d+)\]",
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                );

                if (match.Success && int.TryParse(match.Groups[1].Value, out var id))
                {
                    _logger.LogInformation($"✅ ID du ticket extrait du sujet: {id}");
                    return id;
                }
        
                _logger.LogWarning($"❌ Impossible d'extraire l'ID du sujet: {email.Subject}");
            }

            return null;
        }

        private string ExtractEmailContent(MimeMessage email)
        {
            string content = null;

            // Essayer d'abord le texte brut
            if (!string.IsNullOrEmpty(email.TextBody))
            {
                content = email.TextBody;
                _logger.LogInformation("Contenu extrait du TextBody");
            }
            // Sinon, prendre le HTML et le nettoyer
            else if (!string.IsNullOrEmpty(email.HtmlBody))
            {
                content = StripHtmlTags(email.HtmlBody);
                _logger.LogInformation("Contenu extrait du HtmlBody");
            }

            if (string.IsNullOrEmpty(content))
            {
                return null;
            }

            // Nettoyer le contenu
            return CleanEmailContent(content);
        }

        private string CleanEmailContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                return string.Empty;

            var lines = content.Split('\n');
            var cleanLines = new List<string>();

            foreach (var line in lines)
            {
                var trimmedLine = line.Trim();

                // Arrêter à la première citation (ligne commençant par >)
                if (trimmedLine.StartsWith(">"))
                    break;

                // Arrêter aux signatures communes
                if (trimmedLine.StartsWith("--") ||
                    trimmedLine.StartsWith("___") ||
                    trimmedLine.Contains("Envoyé depuis") ||
                    trimmedLine.Contains("Sent from") ||
                    trimmedLine.Contains("De :") ||
                    trimmedLine.Contains("From:") ||
                    (trimmedLine.Contains("Le ") && trimmedLine.Contains("a écrit")))
                    break;

                cleanLines.Add(line);
            }

            var result = string.Join("\n", cleanLines).Trim();

            // Supprimer les multiples sauts de ligne
            while (result.Contains("\n\n\n"))
            {
                result = result.Replace("\n\n\n", "\n\n");
            }

            return result;
        }

        private string StripHtmlTags(string html)
        {
            if (string.IsNullOrEmpty(html))
                return string.Empty;

            // Simple suppression des balises HTML
            var result = System.Text.RegularExpressions.Regex.Replace(html, "<.*?>", string.Empty);
            
            // Décoder les entités HTML
            result = System.Net.WebUtility.HtmlDecode(result);
            
            return result;
        }
    }
}