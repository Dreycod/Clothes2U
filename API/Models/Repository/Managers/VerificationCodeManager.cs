using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Shared.Enums;

namespace API.Models.Repository.Managers;

public class VerificationCodeManager : GenericCRUDManager<VerificationCode>, IVerificationCodeRepository
{
    public VerificationCodeManager(Clothes2UDbContext context) : base(context)
    {
    }

    public async Task<VerificationCode?> GetLatestCodeAsync(int utilisateurId, VerificationType type)
    {
        return await _context.VerificationCodes
            .Where(vc => vc.UtilisateurId == utilisateurId 
                         && vc.Type == type 
                         && !vc.EstUtilise 
                         && vc.DateExpiration > DateTime.UtcNow)
            .OrderByDescending(vc => vc.DateCreation)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> IsCodeValidAsync(int utilisateurId, string code, VerificationType type)
    {
        return await _context.VerificationCodes
            .AnyAsync(vc => vc.UtilisateurId == utilisateurId
                            && vc.Code == code
                            && vc.Type == type
                            && !vc.EstUtilise
                            && vc.DateExpiration > DateTime.UtcNow
                            && vc.Tentatives < 5);
    }

    public async Task InvalidateOldCodesAsync(int utilisateurId, VerificationType type)
    {
        var oldCodes = await _context.VerificationCodes
            .Where(vc => vc.UtilisateurId == utilisateurId && vc.Type == type && !vc.EstUtilise)
            .ToListAsync();

        foreach (var code in oldCodes)
        {
            code.EstUtilise = true;
        }

        await _context.SaveChangesAsync();
    }
}