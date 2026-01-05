using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class SousCategorieManager : GenericCRUDManager<SousCategorie>
{
    public SousCategorieManager(Clothes2UDbContext context) : base(context){}

    public async Task<IEnumerable<SousCategorie>> GetAllWithDetailsAsync()
    {
        return await BaseSousCategorieQuery().ToListAsync();
    }

    private IQueryable<SousCategorie> BaseSousCategorieQuery()
    {
        return _context.SousCategories
            .Include(sc => sc.Categorie)
            .Include(sc => sc.Mesures)
            .Include(sc => sc.Annonces)
            .AsSplitQuery();
    }
}