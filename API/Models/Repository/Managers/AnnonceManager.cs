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
    public async Task<IEnumerable<Annonce>> GetByUtilisateurFavoris(int id)
    {
        var annonceIds = await _context.Favorises
            .Where(f => f.UtilisateurId == id)
            .Select(f => f.AnnonceId)
            .ToListAsync();

        // Puis récupérer les annonces complètes avec toutes leurs relations
        return await BaseAnnonceQuery()
            .Where(a => annonceIds.Contains(a.AnnonceId))
            .ToListAsync();
    }

    public async Task<IEnumerable<Annonce>> SearchAsync(AnnonceSearchRequestDTO request)
    {
        IQueryable<Annonce> query = BaseAnnonceQuery();

        if (request.CategorieId.HasValue)
            query = query.Where(a => a.CategorieId == request.CategorieId.Value);

        if (request.SousCategorieId.HasValue)
            query = query.Where(a => a.SousCategorieId == request.SousCategorieId.Value);

        if (request.TailleId.HasValue)
            query = query.Where(a => a.TailleId == request.TailleId.Value);

        if (request.EtatId.HasValue)
            query = query.Where(a => a.EtatId == request.EtatId.Value);

        if (request.MarqueId.HasValue)
            query = query.Where(a => a.MarqueId == request.MarqueId.Value);

        if (request.PrixMin.HasValue)
            query = query.Where(a => a.Prix >= request.PrixMin.Value);

        if (request.PrixMax.HasValue)
            query = query.Where(a => a.Prix <= request.PrixMax.Value);

        if (!string.IsNullOrWhiteSpace(request.MotCle))
            query = query.Where(a =>
                a.Title.ToLower().Contains(request.MotCle.ToLower()));

        return await query.ToListAsync();
    }

    public async Task<IEnumerable<Annonce>> GetMostRecentAsync()
    {
        return await BaseAnnonceQuery()
            .OrderByDescending(a => a.DateAnnonce)
            .ToListAsync();
    }

    public async Task<IEnumerable<Annonce>> GetPlusLikeAsync()
    {
        return await BaseAnnonceQuery()
            .OrderByDescending(a => a.UtilisateursFavoris.Count)
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

        if (!string.IsNullOrEmpty(filterDto.Marque))
            query = query.Where(p => p.Marque.NomMarque == filterDto.Marque);

        if (!string.IsNullOrEmpty(filterDto.Categorie))
            query = query.Where(p => p.Categorie.LibelleCategorie == filterDto.Categorie);
        
        if (!string.IsNullOrEmpty(filterDto.SousCategorie))
            query = query.Where(p => p.SousCategorie.LibelleSousCategorie == filterDto.SousCategorie);
        
        if (!string.IsNullOrEmpty(filterDto.Taille))
            query = query.Where(p => p.Taille.Libelletaille == filterDto.Taille);
        
        if (filterDto.Prix.HasValue)
        {
            decimal prixDecimal = (decimal)filterDto.Prix.Value;
            query = query.Where(p => p.Prix <= prixDecimal); 
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