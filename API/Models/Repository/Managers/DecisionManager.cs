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
            .Include(d => d.ElementDecision)
            .ThenInclude(e => e.ElementDecisionAnnonce)
            .Include(d => d.ElementDecision)
            .ThenInclude(e => e.ElementDecisionAvis)
            .Include(d => d.ElementDecision)
            .ThenInclude(e => e.ElementDecisionMessage)
            .Include(d => d.ElementDecision)
            .ThenInclude(e => e.ElementDecisionUtilisateur)
            .AsSplitQuery();
    }

    public async Task<List<Decision>> GetCurrentDecisionSuspensions()
    {
        return await BaseDecisionQuery().Where(d => d.DecisionSanction != null && d.DecisionSanction.EstEnCours == true).ToListAsync();
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

    public async override Task<Decision> GetByIdAsync(int id)
    {
        return await BaseDecisionQuery().FirstOrDefaultAsync(d => d.DecisionId == id);
    }

    public async Task<Decision> GetActiveDecisionByUserId(int id)
    {
        return await BaseDecisionQuery().Where(d => d.UtilisateurId == id && d.DecisionAvertissement == null && d.DecisionSanction.EstEnCours).FirstOrDefaultAsync();   
    }
    public async Task<int> GetDecisionsCountFrom(DateTime date)
    {
        return await _context.Decisions
            .Where(d => d.DecisionDate >= date)
            .CountAsync();
    }

    public async Task<int> GetSuspensionsCountFrom(DateTime date)
    {
        return await _context.Decisions
            .Include(d => d.DecisionSanction.SanctionSuspension)
            .Where(d => d.DecisionDate >= date && d.DecisionSanction.SanctionSuspension != null)
            .CountAsync();
    }

    public async Task<int> GetBannissementsCountFrom(DateTime date)
    {
        return await _context.Decisions
            .Include(d => d.DecisionSanction.SanctionBannissement)
            .Where(d => d.DecisionDate >= date && d.DecisionSanction.SanctionBannissement != null)
            .CountAsync();
    }
}