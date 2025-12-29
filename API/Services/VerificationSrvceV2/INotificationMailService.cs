using API.Models.EntityFramework;

namespace API.Services.VerificationSrvceV2
{
    public interface INotificationMailService
    {
        Task NotifyNewAnnonceAsync(Annonce annonce);
        Task NotifyAnnonceUpdatedAsync(Annonce annonce);
        Task NotifyUserStatusChangedAsync(Utilisateur utilisateur, int oldStatutId);
    }
}
