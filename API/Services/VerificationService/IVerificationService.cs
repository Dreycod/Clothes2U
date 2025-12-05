using API.Models.EntityFramework;

namespace API.Services.Verification;

public interface IVerificationService
{
    Task<(bool success, string message, DateTime? expiresAt)> SendVerificationCodeAsync(int utilisateurId, VerificationType type);
    Task<(bool success, string message)> VerifyCodeAsync(int utilisateurId, string code, VerificationType type);
    string GenerateCode(int length = 6);
}