using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class GenreManager : GenericCRUDManager<Genre>, ICaracteristiquesRepository<Genre>
{
    public GenreManager(Clothes2UDbContext context) : base(context){}
    public async Task<IEnumerable<Genre>> GetAllWithDetailsAsync()
    {
        return await BaseGenreQuery().ToListAsync();
    }

    private IQueryable<Genre> BaseGenreQuery()
    {
        return _context.Genres
            .Include(g => g.Annonces)
            .AsSplitQuery();
    }
}