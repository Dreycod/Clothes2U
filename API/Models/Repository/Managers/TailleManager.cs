using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Shared.DTO.Mesures;

namespace API.Models.Repository.Managers;

public class TailleManager : GenericCRUDManager<Taille>, ITailleRepository
{
    public TailleManager(Clothes2UDbContext context) : base(context) { }

    public async Task<IEnumerable<Taille>> GetAllWithDetailsAsync()
    {
        return await BaseTailleQuery().ToListAsync();
    }

    public async Task<IEnumerable<Mesure>> PutTailleMesuresAsync(int TailleId, List<Mesure> Mesures)
    {
        _context.Mesures.RemoveRange(_context.Mesures.Where(m => m.TailleId == TailleId)); 
        
        for (int i = 0; i < Mesures.Count; i++)
        {
            Mesures[i].TailleId = TailleId;
            _context.Mesures.Add(Mesures[i]);
        }

        _context.SaveChanges(); 

        return Mesures;
    }

    private IQueryable<Taille> BaseTailleQuery()
    {
        return _context.Tailles
            .Include(a => a.Annonces)   
            .Include(a => a.Mesures)
            .AsSplitQuery();
    }
}