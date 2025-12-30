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
        return await BaseDemandeRestaurationQuery().Where(d => d.Status == "En cours").ToListAsync();
    }

    public async Task<DemandeRestauration> GetActiveDemandeRestaurationByUserId(int id)
    {
        return await BaseDemandeRestaurationQuery().FirstOrDefaultAsync(dr => dr.Decision.UtilisateurId == id);
    }
    public override async Task UpdateAsync(DemandeRestauration entity)
    {
        var entityToUpdate = await _context.DemandesRestauration
                                 .FirstOrDefaultAsync(dr => dr.DemandeRestaurationId == entity.GetId())
                             ?? throw new ArgumentException($"Entity with id {entity.GetId()} not found");
            
        _context.DemandesRestauration.Attach(entityToUpdate);
        _context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<int> GetDemandeRestaurationCount()
    {
        return await _context.DemandesRestauration.Where(d => d.Status == "En cours").CountAsync();
    }
}