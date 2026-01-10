using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services.VerificationSrvceV2;

namespace API.Services
{
    public class PasswordResetService
    {
        private readonly IUtilisateurRepository _utilisateurRepo;
        private readonly IPasswordResetRepository<PasswordResetToken, int> _resetRepo;
        private readonly INotificationMailService _mailService;
        private readonly ILoginService _loginService;
        private readonly IConfiguration _config;

        public PasswordResetService(
            IUtilisateurRepository utilisateurRepo,
            IPasswordResetRepository<PasswordResetToken, int> resetRepo,
            INotificationMailService mailService,
            ILoginService loginService,
            IConfiguration config)
        {
            _utilisateurRepo = utilisateurRepo;
            _resetRepo = resetRepo;
            _mailService = mailService;
            _loginService = loginService;
            _config = config;
        }

        public async Task RequestReset(string email)
        {
            var utilisateur = await _utilisateurRepo.GetUtilisateurByEmail(email);

            if (utilisateur == null || !utilisateur.ValidEmail)
                return;

            await _resetRepo.InvalidateUserTokens(utilisateur.UtilisateurId);

            var token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());

            var reset = new PasswordResetToken
            {
                Token = token,
                UtilisateurId = utilisateur.UtilisateurId,
                Expiration = DateTime.UtcNow.AddMinutes(30),
                Used = false
            };

            await _resetRepo.AddAsync(reset);

            var link =
                $"{_config["FrontendUrl"]}/reset-password?token={token}";

            await _mailService.SendPasswordResetEmailAsync(utilisateur, link);
        }

        public async Task ResetPassword(string token, string newPassword)
        {
            var resetToken = await _resetRepo.GetValidToken(token)
                ?? throw new ArgumentException("Token invalide ou expiré");

            var utilisateur = resetToken.UtilisateurReset;

            string hashPassword = _loginService.HashPassword(newPassword);


            await _utilisateurRepo.UpdatePassword(utilisateur.UtilisateurId, hashPassword);

            resetToken.Used = true;

            await _resetRepo.UpdToken();
        }
    }
}
