using API.Models.Repository;
using API.Models.EntityFramework;
using API.Services.VerificationSrvceV2;

namespace API.Services
{
    public class NotificationMailService : INotificationMailService
    {
        private readonly IAbonnementRepository<Abonnement, int> _abonnementRepo;
        private readonly IUtilisateurRepository _utilisateurRepo;
        private readonly IEmailService _emailService;

        public NotificationMailService(
        IAbonnementRepository<Abonnement, int> abonnementRepo,
        IUtilisateurRepository utilisateurRepo,
        IEmailService emailService)
        {
            _abonnementRepo = abonnementRepo;
            _utilisateurRepo = utilisateurRepo;
            _emailService = emailService;
        }

        /// Nouvelle annonce d’un vendeur suivi
        public async Task NotifyNewAnnonceAsync(Annonce annonce)
        {
            var followers = await _abonnementRepo
                .GetAllFollowersByUtilisateurSuivi(annonce.UtilisateurId);

            foreach (var abo in followers)
            {
                var user = abo.UtilisateurSuiveur;

                if (!user.PreferenceNotifMail || !user.ValidEmail)
                    continue;

                await _emailService.SendAsync(
                    user.Email,
                    "Nouvelle annonce disponible",
                    $"Le vendeur {annonce.Utilisateur.Login} a publié une nouvelle annonce : {annonce.Title}"
                );
            }
        }

        /// Modification d’une annonce suivie (favoris)
        public async Task NotifyAnnonceUpdatedAsync(Annonce annonce)
        {
            var users = annonce.UtilisateursFavoris
                .Select(f => f.Utilisateur)
                .Where(u => u.PreferenceNotifMail && u.ValidEmail);

            foreach (var user in users)
            {
                await _emailService.SendAsync(
                    user.Email,
                    "Annonce mise à jour",
                    $"L’annonce '{annonce.Title}' que vous suivez a été modifiée."
                );
            }
        }

        /// Changement de statut utilisateur
        public async Task NotifyUserStatusChangedAsync(Utilisateur utilisateur, int oldStatutId)
        {
            if (!utilisateur.PreferenceNotifMail || !utilisateur.ValidEmail)
                return;

            if (utilisateur.StatutId == 2)
            {

                await _emailService.SendAsync(
                    utilisateur.Email,
                    "Changement de statut de votre compte",
                    "Votre compte a été suspendu. Si vous pensez qu'il s'agit d'une erreur, contactez le support."
                );
            }
            else if (utilisateur.StatutId == 3)
            {
                await _emailService.SendAsync(
                    utilisateur.Email,
                    "Changement de statut de votre compte",
                    "Votre compte a été banni. Si vous pensez qu'il s'agit d'une erreur, contactez le support."
                );
            }

        }

        //// Oublie de mot de passe par mail
        public async Task SendPasswordResetEmailAsync(Utilisateur utilisateur, string resetLink)
        {
            if (!utilisateur.ValidEmail)
                return;
            await _emailService.SendAsync(
                utilisateur.Email,
                "Réinitialisation de votre mot de passe",
                $"Pour réinitialiser votre mot de passe, cliquez sur le lien suivant : {resetLink}"
            );
        }
    }
}
