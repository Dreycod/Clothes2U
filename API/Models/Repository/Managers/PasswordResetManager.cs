using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers
{
    public class PasswordResetManager : GenericCRUDManager<PasswordResetToken>, IPasswordResetRepository<PasswordResetToken, int>
    {


        public PasswordResetManager(Clothes2UDbContext context) : base(context)
        {
        }

        private IQueryable<PasswordResetToken> BaseQuery()
        {
            return _context.PasswordResetTokens
                .Include(t => t.UtilisateurReset)
                .AsSplitQuery();
        }

        public async Task<PasswordResetToken?> GetValidToken(string token)
        {
            return await BaseQuery()
                .FirstOrDefaultAsync(t =>
                    t.Token == token &&
                    !t.Used &&
                    t.Expiration > DateTime.UtcNow);
        }

        public async Task InvalidateUserTokens(int utilisateurId)
        {
            var tokens = await _context.PasswordResetTokens
                .Where(t => t.UtilisateurId == utilisateurId && !t.Used)
                .ToListAsync();

            foreach (var token in tokens)
                token.Used = true;

            await _context.SaveChangesAsync();
        }

        public async Task UpdToken()
        {
            await _context.SaveChangesAsync();
        }
    }
}
