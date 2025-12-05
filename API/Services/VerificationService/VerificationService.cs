using API.Models.EntityFramework;
using API.Models.Repository;
using API.Services.Email;
using API.Services.SMS;

namespace API.Services.Verification;

public class VerificationService : IVerificationService
{
    private readonly IVerificationCodeRepository _verificationCodeRepository;
    private readonly IUtilisateurRepository _utilisateurManager;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly ILogger<VerificationService> _logger;

    public VerificationService(
        IVerificationCodeRepository verificationCodeRepository,
        IUtilisateurRepository utilisateurRepository,
        IEmailService emailService,
        ISmsService smsService,
        ILogger<VerificationService> logger)
    {
        _verificationCodeRepository = verificationCodeRepository;
        _utilisateurManager = utilisateurRepository;
        _emailService = emailService;
        _smsService = smsService;
        _logger = logger;
    }

    public async Task<(bool success, string message, DateTime? expiresAt)> SendVerificationCodeAsync(
        int utilisateurId, 
        VerificationType type)
    {
        try
        {
            var utilisateur = await _utilisateurManager.GetByIdAsync(utilisateurId);
            if (utilisateur == null)
                return (false, "Utilisateur introuvable", null);

            if (type == VerificationType.Email && utilisateur.ValidEmail)
                return (false, "Email déjà vérifié", null);

            if (type == VerificationType.Telephone && utilisateur.ValidTelephone)
                return (false, "Téléphone déjà vérifié", null);

            await _verificationCodeRepository.InvalidateOldCodesAsync(utilisateurId, type);

            var code = GenerateCode();
            var expiresAt = DateTime.UtcNow.AddMinutes(10);

            var verificationCode = new VerificationCode
            {
                UtilisateurId = utilisateurId,
                Code = code,
                Type = type,
                DateCreation = DateTime.UtcNow,
                DateExpiration = expiresAt,
                EstUtilise = false,
                Tentatives = 0
            };

            await _verificationCodeRepository.AddAsync(verificationCode);

            bool sent = false;
            if (type == VerificationType.Email)
            {
                sent = await _emailService.SendVerificationEmailAsync(utilisateur.Email, code);
            }
            else if (type == VerificationType.Telephone)
            {
                if (string.IsNullOrEmpty(utilisateur.Telephone))
                    return (false, "Numéro de téléphone manquant", null);
                
                sent = await _smsService.SendVerificationSmsAsync(utilisateur.Telephone, code);
            }

            if (!sent)
                return (false, "Erreur lors de l'envoi du code", null);

            return (true, "Code envoyé avec succès", expiresAt);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'envoi du code de vérification");
            return (false, "Erreur serveur", null);
        }
    }

    public async Task<(bool success, string message)> VerifyCodeAsync(
        int utilisateurId, 
        string code, 
        VerificationType type)
    {
        try
        {
            var verificationCode = await _verificationCodeRepository.GetLatestCodeAsync(utilisateurId, type);

            if (verificationCode == null)
                return (false, "Aucun code de vérification trouvé");

            if (verificationCode.DateExpiration < DateTime.UtcNow)
                return (false, "Code expiré");

            if (verificationCode.Tentatives >= 5)
                return (false, "Trop de tentatives. Demandez un nouveau code");

            // Récupérer l'entité depuis la DB pour la mise à jour
            var existingCode = await _verificationCodeRepository.GetByIdAsync(verificationCode.VerificationCodeId);
            if (existingCode == null)
                return (false, "Code introuvable");

            // Incrémenter les tentatives
            existingCode.Tentatives++;
            await _verificationCodeRepository.UpdateAsync(existingCode, existingCode);

            if (verificationCode.Code != code)
                return (false, $"Code incorrect ({5 - existingCode.Tentatives} tentatives restantes)");

            // Code correct : marquer comme utilisé
            existingCode.EstUtilise = true;
            await _verificationCodeRepository.UpdateAsync(existingCode, existingCode);

            // Valider l'utilisateur
            var utilisateur = await _utilisateurManager.GetByIdAsync(utilisateurId);
            if (utilisateur != null)
            {
                var utilisateurToUpdate = await _utilisateurManager.GetByIdAsync(utilisateurId);
                
                if (type == VerificationType.Email)
                    utilisateurToUpdate.ValidEmail = true;
                else if (type == VerificationType.Telephone)
                    utilisateurToUpdate.ValidTelephone = true;

                await _utilisateurManager.UpdateAsync(utilisateur, utilisateurToUpdate);
            }

            return (true, "Vérification réussie");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la vérification du code");
            return (false, "Erreur serveur");
        }
    }

    public string GenerateCode(int length = 6)
    {
        var random = new Random();
        var code = string.Empty;
        
        for (int i = 0; i < length; i++)
        {
            code += random.Next(0, 10).ToString();
        }
        
        return code;
    }
}