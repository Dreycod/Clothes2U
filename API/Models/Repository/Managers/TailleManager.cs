using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class TailleManager : GenericCRUDManager<Taille>, IFiltrableByIdRepository<Taille, int>
{
    public TailleManager(Clothes2UDbContext context) : base(context){}

    public async Task<IEnumerable<Taille>> GetAllAsyncByIdentifier(int id)
    {
        return await _context.Tailles.Where(e => e.CategorieTailleId == id).ToListAsync();
    }
}