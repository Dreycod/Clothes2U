using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class DemandeRestaurationManager : GenericCRUDManager<DemandeRestauration>, IDemandeRestaurationRepository<DemandeRestauration, int>
{
    public DemandeRestaurationManager(Clothes2UDbContext context) : base(context){}

    private IQueryable<DemandeRestauration> BaseDemandeRestaurationQuery()
    {
        return _context.DemandesRestauration
            .Include(dr => dr.Decision)
            .ThenInclude(d => d.Utilisateur)
            .AsSplitQuery();
    }

    public async override Task<DemandeRestauration?> GetByIdAsync(int id)
    {
        return await BaseDemandeRestaurationQuery().FirstOrDefaultAsync(dr => dr.DemandeRestaurationId == id);
    }

    public async override Task<IEnumerable<DemandeRestauration>> GetAllAsync()
    {
        return await BaseDemandeRestaurationQuery().ToListAsync();
    }

    public async Task<DemandeRestauration> GetActiveDemandeRestaurationByUserId(int id)
    {
        return await BaseDemandeRestaurationQuery().FirstOrDefaultAsync(dr => dr.Decision.UtilisateurId == id);
    }
}