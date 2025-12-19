using API.Models.EntityFramework;
using Shared.Enums;

namespace API.Models.Repository;

public interface IVerificationCodeRepository : IDataRepository<VerificationCode, int>
{
    Task<VerificationCode?> GetLatestCodeAsync(int utilisateurId, VerificationType type);
    Task<bool> IsCodeValidAsync(int utilisateurId, string code, VerificationType type);
    Task InvalidateOldCodesAsync(int utilisateurId, VerificationType type);
}