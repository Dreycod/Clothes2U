using Shared.DTO.Tag;
using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers
{
    public class TagManager : GenericCRUDManager<Tag>, ITagRepository<Tag, int>
    {
        public TagManager(Clothes2UDbContext context) : base(context) { }

        private IQueryable<Tag> BaseQuery()
        {
            return _context.Tags
                .Include(t => t.Annonces)
                    .ThenInclude(r => r.Annonce)
                .AsSplitQuery();
        }

        public async Task<Tag?> GetByTagId(int id)
        {
            return await BaseQuery()
                .FirstOrDefaultAsync(t => t.TagId == id);
        }

        public async Task<IEnumerable<Tag>> GetAllTagsAsync()
        {
            return await BaseQuery()
                .OrderBy(t => t.LibelleTag)
                .ToListAsync();
        }

        public async Task<IEnumerable<Tag>> SearchAsync(CreateTagDTO request)
        {
            IQueryable<Tag> query = BaseQuery();

            if (!string.IsNullOrWhiteSpace(request.Libelle))
            {
                string mot = request.Libelle.Trim().ToLower();
                query = query.Where(t => t.LibelleTag.ToLower().Contains(mot));
            }

            return await query
                .OrderBy(t => t.LibelleTag)
                .ToListAsync();
        }
    }
}