using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services.VerificationSrvceV2;
using Shared.DTO.Mail;

namespace API.Services
{
    public class NotificationMailService : INotificationMailService
    {
        private readonly IEmailService _emailService;
        private readonly IEmailTemplateService _templateService;

        public NotificationMailService(
            IEmailService emailService,
            IEmailTemplateService templateService)
        {
            _emailService = emailService;
            _templateService = templateService;
        }
        public async Task NotifyNewAnnonceAsync(string annonceTitle, string UtilisateurLogin, int annonceId, string userMail)
        {
            var htmlContent = _templateService.GetNewAnnonceTemplate(
                "Utilisateur", 
                UtilisateurLogin,
                annonceTitle,
                $"https://votre-site.com/annonce/{annonceId}"
            );

            await _emailService.SendHtmlAsync(
                userMail,
                "Nouvelle annonce disponible",
                htmlContent
            );
        }
        public async Task NotifyAnnonceUpdatedAsync(string annonceTitle, int annonceId, string userMail)
        {
            var htmlContent = _templateService.GetAnnonceUpdatedTemplate(
                "Utilisateur",
                annonceTitle,
                $"https://votre-site.com/annonce/{annonceId}"
            );

            await _emailService.SendHtmlAsync(
                userMail,
                "Annonce mise à jour",
                htmlContent
            );
        }
        public async Task NotifyUserStatusChangedAsync(Utilisateur utilisateur, int oldStatutId)
        {
            if (!utilisateur.PreferenceNotifMail || !utilisateur.ValidEmail)
                return;

            string htmlContent;
            string subject;

            if (utilisateur.StatutId == 2)
            {
                subject = "Compte suspendu";
                htmlContent = _templateService.GetAccountSuspendedTemplate(utilisateur.Login);
            }
            else if (utilisateur.StatutId == 3)
            {
                subject = "Compte banni";
                htmlContent = _templateService.GetAccountBannedTemplate(utilisateur.Login);
            }
            else
            {
                return;
            }

            await _emailService.SendHtmlAsync(
                utilisateur.Email,
                subject,
                htmlContent
            );
        }
        public async Task SendPasswordResetEmailAsync(Utilisateur utilisateur, string resetLink)
        {
            if (!utilisateur.ValidEmail)
                return;

            var htmlContent = _templateService.GetPasswordResetTemplate(
                utilisateur.Login,
                resetLink
            );

            await _emailService.SendHtmlAsync(
                utilisateur.Email,
                "Réinitialisation de votre mot de passe",
                htmlContent
            );
        }

        public async Task SendSupportMailAsync(string userMail, string userName, int ticketId, MailDTO mail)
        {
            var htmlContent = _templateService.GetSupportResponseTemplate(
                userName,
                mail.MailObject,
                mail.MailContent,
                ticketId
            );
            var subject = $"[Ticket #{ticketId}] {mail.MailObject}";
            await _emailService.SendHtmlAsync(
                userMail,
                subject,
                htmlContent
            );
        }

        public async Task SendErrorTicketClosed(string userMail, string userName)
        {
            var htmlContent = _templateService.GetClosedTicketTemplate(
                userName
            );
            var subject = $"Erreur d'envoie";
            await _emailService.SendHtmlAsync(
                userMail,
                subject,
                htmlContent
            );
        }
    }
}