using API.Models.EntityFramework;
using API.Services.VerificationSrvceV2;
using API.Models.Repository;
using Shared.Enums;

namespace API.Services.VerificationSrvceV2
{
    public class VerificationService : IVerificationService
    {
        private readonly IVerificationCodeRepository _codeRepo;
        private readonly IUtilisateurRepository _userRepo;
        private readonly ISmsService _sms;
        private readonly IEmailService _email;
        private readonly IConfiguration _config;

        public VerificationService(
       IVerificationCodeRepository codeRepo,
       IUtilisateurRepository userRepo,
       ISmsService sms,
       IEmailService email,
       IConfiguration config)
        {
            _codeRepo = codeRepo;
            _userRepo = userRepo;
            _sms = sms;
            _email = email;
            _config = config;
        }

        public async Task<(bool, string, DateTime?)> SendVerificationCodeAsync(
         int userId,
         VerificationType type,
         string? phoneNumber)
        {
            var user = await _userRepo.GetByIdAsync(userId);
            if (user == null)
                return (false, "Utilisateur introuvable", null);

            // SI TELEPHONE ET MANQUANT → ON L’ENREGISTRE
            if (type == VerificationType.Telephone)
            {
                if (string.IsNullOrWhiteSpace(user.Telephone))
                {
                    if (string.IsNullOrWhiteSpace(phoneNumber))
                        return (false, "Numéro de téléphone requis", null);

                    user.Telephone = phoneNumber;
                    await _userRepo.UpdateAsync(user);
                }
            }

            await _codeRepo.InvalidateOldCodesAsync(userId, type);

            var code = GenerateCode();
            var expiration = DateTime.UtcNow.AddMinutes(
                int.Parse(_config["Verification:ExpirationMinutes"])
            );

            var entity = new VerificationCode
            {
                UtilisateurId = userId,
                Code = code,
                Type = type,
                DateExpiration = expiration
            };

            await _codeRepo.AddAsync(entity);

            // ENVOI
            if (type == VerificationType.Telephone)
            {
                await _sms.SendAsync(
                    user.Telephone!,
                    $"Votre code Clothes2U est : {code}"
                );
            }
            else
            {
                await _email.SendAsync(
                    user.Email,
                    "Code de vérification Clothes2U",
                    $"Votre code est : {code}"
                );
            }

            return (true, "Code envoyé", expiration);
        }


        public async Task<(bool, string)> VerifyCodeAsync(
            int userId, string code, VerificationType type)
        {
            var valid = await _codeRepo.IsCodeValidAsync(userId, code, type);
            if (!valid)
                return (false, "Code invalide ou expiré");

            var entity = await _codeRepo.GetLatestCodeAsync(userId, type);
            entity!.EstUtilise = true;

            var user = await _userRepo.GetByIdAsync(userId);
            if (type == VerificationType.Telephone)
                user.ValidTelephone = true;
            else
                user.ValidEmail = true;

            await _userRepo.UpdateAsync(user);
            return (true, "Vérification réussie");
        }

        private string GenerateCode()
        {
            var length = int.Parse(_config["Verification:CodeLength"]);
            return Random.Shared.Next((int)Math.Pow(10, length - 1),
                                      (int)Math.Pow(10, length)).ToString();
        }
    }
}
