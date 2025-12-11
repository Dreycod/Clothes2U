using API.DTO.Recense;
using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers
{
    public class RecenseManager : GenericCRUDManager<Recense>, IRecenseRepository<Recense, int>
    {
        public RecenseManager(Clothes2UDbContext context) : base(context) { }

        private IQueryable<Recense> BaseQuery()
        {
            return _context.Recenses
                .Include(r => r.Annonce)
                .Include(r => r.Tag)
                .AsSplitQuery();
        }

        public async Task<IEnumerable<Recense>> GetByRecenseId(int id)
        {
            return await BaseQuery()
                .Where(r => r.RecenseId == id)
                .ToListAsync();
        }

        public async Task<IEnumerable<Recense>> GetAllAnnonceByTagId(int tagId)
        {
            return await BaseQuery()
                .Where(r => r.TagId == tagId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Recense>> GetAllTagByAnnonceId(int annonceId)
        {
            return await BaseQuery()
                .Where(r => r.AnnonceId == annonceId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Recense>> SearchAsync(RecenseSearchRequestDTO request)
        {
            IQueryable<Recense> query = BaseQuery();

            if (request.TagId.HasValue)
                query = query.Where(r => r.TagId == request.TagId.Value);

            if (!string.IsNullOrWhiteSpace(request.MotCle))
            {
                string mot = request.MotCle.Trim().ToLower();
                query = query.Where(r =>
                    r.Tag.LibelleTag.ToLower().Contains(mot));
            }

            return await query.ToListAsync();
        }
    }
}
