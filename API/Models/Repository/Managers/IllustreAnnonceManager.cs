using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class IllustreAnnonceManager : GenericCRUDManager<Illustre_Annonce>, IllustreAnnonceRepository<Illustre_Annonce, int>
{
    public IllustreAnnonceManager(Clothes2UDbContext context) : base(context){}

    public async Task<Illustre_Annonce?> GetByPhotoId(int photoId)
    {
        return await _context.Illustre_Annonces.FirstOrDefaultAsync(i => i.PhotoId == photoId);
    }
    public async Task<IEnumerable<Illustre_Annonce>?> GetByPhotoIds(IEnumerable<int> photoIds)
    {
        return await _context.Illustre_Annonces
                             .Where(i => photoIds.Contains(i.PhotoId))
                             .ToListAsync();
    }
}