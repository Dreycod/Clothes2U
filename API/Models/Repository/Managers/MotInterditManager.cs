using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class MotInterditManager : GenericCRUDManager<MotInterdit>, IMotInterditRepository
{
    public MotInterditManager(Clothes2UDbContext context) : base(context) {}

    public async Task<bool> EstInterdit(string mot)
    {
        MotInterdit? motInterdit = await _context.MotsInterdits.FirstOrDefaultAsync(m => m.LibelleMot == mot);
        return motInterdit != null;
    }
}