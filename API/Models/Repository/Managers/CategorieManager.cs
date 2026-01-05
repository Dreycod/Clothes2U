using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class CategorieManager : GenericCRUDManager<Categorie>, ICaracteristiquesRepository<Categorie>
{
    public CategorieManager(Clothes2UDbContext context) : base(context){}

    public async Task<IEnumerable<Categorie>> GetAllWithDetailsAsync()
    {
        return await BaseCategorieQuery().ToListAsync();
    }

    private IQueryable<Categorie> BaseCategorieQuery()
    {
        return _context.Categories
            .Include(a => a.Annonces)
            .Include(a => a.SousCategories)
            .AsSplitQuery();
    }
}