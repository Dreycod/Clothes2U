using API.DTO.Annonce;
using API.Extensions;
using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class AnnonceManager : GenericCRUDManager<Annonce>, IAnnonceRepository<Annonce, int>
{
    public AnnonceManager(Clothes2UDbContext context) : base(context)
    {
    }
    
    private IQueryable<Annonce> BaseAnnonceQuery()
    {
        return _context.Annonces
            .Include(a => a.Marque)
            .Include(a => a.Statut)
            .Include(a => a.Taille)
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
        return await BaseAnnonceQuery()
            .Where(a => a.UtilisateursFavoris
                .Any(f => f.UtilisateurId == id))
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
                a.Title.Contains(request.MotCle));

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




}