using API.Models.EntityFramework;
using Shared.Enums;

namespace API.Services.VerificationSrvceV2
{
    public interface IVerificationService
    {
        Task<(bool success, string message, DateTime? expiresAt)>
        SendVerificationCodeAsync(int userId, VerificationType type, string? phoneNumber);

        Task<(bool success, string message)>
        VerifyCodeAsync(int userId, string code, VerificationType type);
    }
}
