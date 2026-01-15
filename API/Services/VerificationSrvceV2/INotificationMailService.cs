using API.Models.EntityFramework;
using Shared.DTO.Mail;

namespace API.Services.VerificationSrvceV2
{
    public interface INotificationMailService
    {
        Task NotifyNewAnnonceAsync(string annonceTitle, string UtilisateurLogin, int annonceId, string userMail);
        Task NotifyAnnonceUpdatedAsync(string annonceTitle, int annonceId, string userMail);
        Task NotifyUserStatusChangedAsync(Utilisateur utilisateur, int oldStatutId);
        Task SendPasswordResetEmailAsync(Utilisateur utilisateur, string resetLink);
        Task SendSupportMailAsync(string userMail, string userName, int ticketId, MailDTO mail);
        Task SendErrorTicketClosed(string userMail, string userName);
    }
}
