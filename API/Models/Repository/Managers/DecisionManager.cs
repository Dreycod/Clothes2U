using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class DecisionManager : GenericCRUDManager<Decision>, IDecisionRepository
{
    public DecisionManager(Clothes2UDbContext context) :  base(context){}

    private IQueryable<Decision> BaseDecisionQuery()
    {
        return _context.Decisions
            .Include(d => d.Utilisateur)
            .Include(d => d.Moderateur)
            .Include(d => d.DecisionAvertissement)
            .Include(d => d.DecisionSanction)
            .ThenInclude(s => s.SanctionBannissement)
            .Include(d => d.DecisionSanction)
            .ThenInclude(s => s.SanctionSuspension)
            .AsSplitQuery();
    }

    public async Task<IEnumerable<Decision>> GetAllDecisionsByModerateurId(int id)
    {
        return await BaseDecisionQuery().Where(d => d.ModerateurId == id).ToListAsync();
    }
    public async Task<Decision> AddAsync(Decision decision)
    {
        _context.Decisions.Add(decision);
        await _context.SaveChangesAsync();
        return decision;
    }

    public async Task<IEnumerable<Decision>> GetAllDecisionByUserId(int id)
    {
        return BaseDecisionQuery().Where(d => d.UtilisateurId == id).ToList();   
    }

    public async Task<IEnumerable<Decision>> GetAllActiveDecisionByUserId(int id)
    {
        return BaseDecisionQuery().Where(d => d.UtilisateurId == id && d.DecisionAvertissement == null && d.DecisionSanction.EstEnCours).ToList();
    }
}