using Shared.DTO;
using Shared.DTO.Annonce;
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
            .Include(a => a.GenreAnnonce)
            .Include(a => a.Tags)
            .ThenInclude(t => t.Tag)
            .Include(a => a.Etat)
            .Include(a => a.Photos)
            .ThenInclude(pa => pa.Photo)
            .Include(a => a.UtilisateursFavoris)
            .Include(a => a.Utilisateur)
            .ThenInclude(u => u.PhotoProfil)
            .Include(a => a.Utilisateur)
            .ThenInclude(u => u.Statut) 
            .Include(a => a.Couleurs)
            .ThenInclude(c => c.Couleur)
            .Include(a => a.LesVisualisations)
            .AsSplitQuery(); 
    }

    public override async Task<Annonce?> GetByIdAsync(int id)
    {
        return await BaseAnnonceQuery()
            .FirstOrDefaultAsync(a => a.AnnonceId == id);
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

    public async Task<IEnumerable<Annonce>> GetByUtilisateurFavoris(int id)
    {
        var annonceIds = await _context.Favorises
            .Where(f => f.UtilisateurId == id)
            .Select(f => f.AnnonceId)
            .ToListAsync();

        return await BaseAnnonceQuery()
            .AsSingleQuery() 
            .Where(a => annonceIds.Contains(a.AnnonceId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Annonce>> FilterAsync(FilterDTO filterDto, int page, int pageSize, int? currentUserId = null)
    {
        var query = BaseAnnonceQuery();
        if (currentUserId.HasValue)
        {
            query = query.Where(a => !_context.Bloques
                .Any(b => b.UtilisateurBloqueurId == currentUserId.Value && 
                          b.UtilisateurBloqueId == a.UtilisateurId));
        }
        if (!string.IsNullOrEmpty(filterDto.MotCle))
        {
            var lowerMotCle = filterDto.MotCle.ToLower();
            query = query.Where(p =>
                p.Title.ToLower().Contains(lowerMotCle) ||
                p.Tags.Any(t => t.Tag.LibelleTag.ToLower().Contains(lowerMotCle))
            );
        }

        query = query.Where(p => p.Utilisateur.Statut.StatutLibelle == "Actif");
        if (filterDto.Marques != null && filterDto.Marques.Any())
        {
            query = query.Where(p => filterDto.Marques.Contains(p.Marque.NomMarque));
        }
        
        if (filterDto.Etats != null && filterDto.Etats.Any())
        {
            query = query.Where(p => filterDto.Etats.Contains(p.Etat.NomEtat));
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
        
        if (filterDto.Genre != null && filterDto.Genre.Any())
        {
            query = query.Where(p => filterDto.Genre.Contains(p.GenreAnnonce.NomGenre));
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

        int skip = (page - 1) * pageSize;
        query = query.Skip(skip).Take(pageSize);

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

    public async Task SuspendElement(int id)
    {
        Console.WriteLine("----------------------------------------------------> et oui on est la");
        Annonce annonce = _context.Annonces.Find(id);
        annonce.StatutAnnonceId = 2;
        _context.Annonces.Update(annonce);
        await _context.SaveChangesAsync();
    }
}