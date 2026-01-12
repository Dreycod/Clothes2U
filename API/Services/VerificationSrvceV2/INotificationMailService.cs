using API.Models.EntityFramework;
using Shared.DTO.Mail;

namespace API.Services.VerificationSrvceV2
{
    public interface INotificationMailService
    {
        Task NotifyNewAnnonceAsync(Annonce annonce, string userMail);
        Task NotifyAnnonceUpdatedAsync(Annonce annonce, string userMail);
        Task NotifyUserStatusChangedAsync(Utilisateur utilisateur, int oldStatutId);
        Task SendPasswordResetEmailAsync(Utilisateur utilisateur, string resetLink);
        Task SendSupportMailAsync(string userMail, MailDTO mail);
    }
}
