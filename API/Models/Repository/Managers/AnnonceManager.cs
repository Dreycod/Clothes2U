using API.DTO;
using API.DTO.Annonce;
using API.Extensions;
using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class AnnonceManager : GenericCRUDManager<Annonce>, IAnnonceRepository<Annonce, int, FilterDTO>
{
    public AnnonceManager(Clothes2UDbContext context) : base(context)
    {
    }
    
    private IQueryable<Annonce> BaseAnnonceQuery()
    {
        return _context.Annonces
            .Include(a => a.Marque)
            .Include(a => a.Statut)
            .Include(a => a.Categorie)
            .Include(a => a.SousCategorie)
            .Include(a => a.Taille)
            .Include(a => a.Tags)
            .ThenInclude(t => t.Tag)
            .Include(a => a.Etat)
            .Include(a => a.Photos)
            .ThenInclude(pa => pa.Photo)
            .Include(a => a.UtilisateursFavoris)
            .Include(a => a.Utilisateur)
            .ThenInclude(u => u.PhotoProfil)
            .AsSplitQuery(); 
    }

    public override async Task<Annonce?> GetByIdAsync(int id)
    {
        return await BaseAnnonceQuery()
            .FirstOrDefaultAsync(a => a.AnnonceId == id);
    }


    public async Task<IEnumerable<Annonce>> GetByCategorieId(int id)
    {
        return await BaseAnnonceQuery()
            .Where(a => a.CategorieId == id)
            .ToListAsync();
    }

    public async Task<IEnumerable<Annonce>> GetBySousCategorieId(int id)
    {
        return await BaseAnnonceQuery()
            .Where(a => a.SousCategorieId == id)
            .ToListAsync();
    }

    public async Task<IEnumerable<Annonce>> GetByUtilisateurId(int id)
    {
        return await BaseAnnonceQuery()
            .Where(a => a.UtilisateurId == id)
            .ToListAsync();
    }
    public async Task<IEnumerable<Annonce>> GetActiveAnnonces()
    {
        return await BaseAnnonceQuery()
            .Where(a => a.Statut.StatutLibelle == "En Ligne") 
            .ToListAsync();
    }

    public async Task<IEnumerable<Annonce>> GetRecentAnnonces()
    {
        var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

        return await BaseAnnonceQuery()
            .Where(a => a.DateAnnonce >= thirtyDaysAgo)
            .ToListAsync();
    }

    public async Task<IEnumerable<Annonce>> GetByUtilisateurFavoris(int id)
    {
        var annonceIds = await _context.Favorises
            .Where(f => f.UtilisateurId == id)
            .Select(f => f.AnnonceId)
            .ToListAsync();

        return await BaseAnnonceQuery()
            .Where(a => annonceIds.Contains(a.AnnonceId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Annonce>> FilterAsync(FilterDTO filterDto)
    {
        var query = BaseAnnonceQuery();
    
        if (!string.IsNullOrEmpty(filterDto.MotCle))
        {
            var lowerMotCle = filterDto.MotCle.ToLower();
            query = query.Where(p =>
                p.Title.ToLower().Contains(lowerMotCle) ||
                p.Tags.Any(t => t.Tag.LibelleTag.ToLower().Contains(lowerMotCle))
            );
        }
        if (filterDto.Marques != null && filterDto.Marques.Any())
        {
            query = query.Where(p => filterDto.Marques.Contains(p.Marque.NomMarque));
        }
        if (filterDto.Categories != null && filterDto.Categories.Any())
        {
            query = query.Where(p => filterDto.Categories.Contains(p.Categorie.LibelleCategorie));
        }
        if (filterDto.SousCategories != null && filterDto.SousCategories.Any())
        {
            query = query.Where(p => filterDto.SousCategories.Contains(p.SousCategorie.LibelleSousCategorie));
        }
        if (filterDto.Tailles != null && filterDto.Tailles.Any())
        {
            query = query.Where(p => filterDto.Tailles.Contains(p.Taille.Libelletaille));
        }
        if (filterDto.PrixMin.HasValue)
        {
            decimal prixMinDecimal = (decimal)filterDto.PrixMin.Value;
            query = query.Where(p => p.Prix >= prixMinDecimal);
        }
    
        if (filterDto.PrixMax.HasValue)
        {
            decimal prixMaxDecimal = (decimal)filterDto.PrixMax.Value;
            query = query.Where(p => p.Prix <= prixMaxDecimal);
        }
        query = ApplySorting(query, filterDto);

        return await query.ToListAsync();
    }

    private IQueryable<Annonce> ApplySorting(IQueryable<Annonce> query, FilterDTO filterDto)
    {
        if (filterDto.SortBy == null)
            return query.OrderByDescending(a => a.DateAnnonce); 

        var isDescending = filterDto.SortOrder == SortOrder.Descending;

        return filterDto.SortBy switch
        {
            SortField.Prix => isDescending 
                ? query.OrderByDescending(a => a.Prix) 
                : query.OrderBy(a => a.Prix),
                
            SortField.DateAnnonce => isDescending 
                ? query.OrderByDescending(a => a.DateAnnonce) 
                : query.OrderBy(a => a.DateAnnonce),
                
            SortField.Titre => isDescending 
                ? query.OrderByDescending(a => a.Title) 
                : query.OrderBy(a => a.Title),
                
            SortField.NombreFavoris => isDescending 
                ? query.OrderByDescending(a => a.UtilisateursFavoris.Count) 
                : query.OrderBy(a => a.UtilisateursFavoris.Count),
                
            _ => query.OrderByDescending(a => a.DateAnnonce)
        };
    }



}