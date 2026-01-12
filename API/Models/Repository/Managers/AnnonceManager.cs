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
            .Where(a => a.Statut.StatutLibelle == "En Ligne" && a.Utilisateur.Statut.StatutLibelle == "Actif") 
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

    public async Task<IEnumerable<Annonce>> FilterAsync(FilterDTO filterDto, int page, int pageSize, int? currentUserId = null)
{
    var query = BaseAnnonceQuery();
    
    // ✅ Charger les IDs des utilisateurs bloqués AVANT la requête principale
    List<int> blockedUserIds = new List<int>();
    if (currentUserId.HasValue)
    {
        blockedUserIds = await _context.Bloques
            .Where(b => b.UtilisateurBloqueurId == currentUserId.Value)
            .Select(b => b.UtilisateurBloqueId)
            .ToListAsync();
        
        // ✅ Filtrer avec la liste en mémoire
        if (blockedUserIds.Any())
        {
            query = query.Where(a => !blockedUserIds.Contains(a.UtilisateurId));
        }
    }
    
    if (!string.IsNullOrEmpty(filterDto.MotCle))
    {
        var lowerMotCle = filterDto.MotCle.ToLower();
        query = query.Where(p =>
            p.Title.ToLower().Contains(lowerMotCle) ||
            p.Tags.Any(t => t.Tag.LibelleTag.ToLower().Contains(lowerMotCle)) ||
            p.Categorie.LibelleCategorie.ToLower().Contains(lowerMotCle) || 
            p.SousCategorie.LibelleSousCategorie.ToLower().Contains(lowerMotCle) || 
            p.Couleurs.Any(c => c.Couleur.Nom.ToLower().Contains(lowerMotCle)) ||
            p.Marque.NomMarque.ToLower().Contains(lowerMotCle)
        );
    }

    query = query.Where(p => p.Utilisateur.Statut.StatutLibelle == "Actif" && p.Statut.StatutLibelle == "En Ligne");
    
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

public async Task<IEnumerable<Annonce>> GetSimilarAsync(int annonceId, int page, int pageSize, int? currentUserId = null)
{
    var referenceData = await _context.Annonces
        .AsNoTracking()
        .Where(a => a.AnnonceId == annonceId)
        .Select(a => new
        {
            a.SousCategorieId,
            a.CategorieId,
            a.MarqueId,
            a.GenreId,
            a.TailleId,
            a.EtatId,
            a.Prix,
            TagIds = a.Tags.Select(t => t.TagId).ToList(),
            CouleurIds = a.Couleurs.Select(c => c.CouleurId).ToList()
        })
        .FirstOrDefaultAsync();
    
    if (referenceData == null)
        return Enumerable.Empty<Annonce>();

    var query = BaseAnnonceQuery()
        .Where(a => a.AnnonceId != annonceId)
        .Where(a => a.Statut.StatutLibelle == "En Ligne")
        .Where(a => a.Utilisateur.Statut.StatutLibelle == "Actif");

    // ✅ Charger les IDs des utilisateurs bloqués AVANT
    List<int> blockedUserIds = new List<int>();
    if (currentUserId.HasValue)
    {
        blockedUserIds = await _context.Bloques
            .Where(b => b.UtilisateurBloqueurId == currentUserId.Value)
            .Select(b => b.UtilisateurBloqueId)
            .ToListAsync();
        
        // ✅ Filtrer avec la liste en mémoire
        if (blockedUserIds.Any())
        {
            query = query.Where(a => !blockedUserIds.Contains(a.UtilisateurId));
        }
    }
    
    var annoncesWithBasicScore = await query
        .Select(a => new
        {
            Annonce = a,
            BasicScore = 
                (a.SousCategorieId == referenceData.SousCategorieId ? 50 : 0) +
                (a.CategorieId == referenceData.CategorieId ? 30 : 0) +
                (a.MarqueId == referenceData.MarqueId ? 20 : 0) +
                (a.GenreId == referenceData.GenreId ? 15 : 0) +
                (a.TailleId == referenceData.TailleId ? 10 : 0) +
                (a.EtatId == referenceData.EtatId ? 10 : 0) +
                (a.Prix >= referenceData.Prix * 0.7m && 
                 a.Prix <= referenceData.Prix * 1.3m ? 15 : 0)
        })
        .Where(x => x.BasicScore > 0)
        .OrderByDescending(x => x.BasicScore)
        .ThenByDescending(x => x.Annonce.DateAnnonce)
        .Take(pageSize * 3)
        .ToListAsync();
        
    var annoncesWithFullScore = annoncesWithBasicScore
        .Select(item => new
        {
            item.Annonce,
            FullScore = item.BasicScore +
                item.Annonce.Tags.Count(at => referenceData.TagIds.Contains(at.TagId)) * 5 +
                item.Annonce.Couleurs.Count(ac => referenceData.CouleurIds.Contains(ac.CouleurId)) * 3
        })
        .OrderByDescending(x => x.FullScore)
        .ThenByDescending(x => x.Annonce.DateAnnonce)
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .Select(x => x.Annonce)
        .ToList();

    return annoncesWithFullScore;
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
        Annonce annonce = _context.Annonces.Find(id);
        annonce.StatutAnnonceId = 2;
        _context.Annonces.Update(annonce);
        await _context.SaveChangesAsync();
    }
}