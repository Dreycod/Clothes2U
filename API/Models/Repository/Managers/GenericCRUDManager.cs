using API.Extensions;
using API.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public abstract class GenericCRUDManager<T> : IDataRepository<T, int> where T : class, IEntity
{
    protected readonly Clothes2UDbContext _context;

    protected GenericCRUDManager(Clothes2UDbContext context)
    {
        _context = context;
    }
    
    
    public virtual async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _context.Set<T>().IncludeNavigationPropertiesIfNeeded().ToListAsync();
    }


    public virtual async Task<T?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public virtual async Task AddAsync(T entity)
    {
        throw new NotImplementedException();
    }

    public virtual async  Task UpdateAsync(T entityToUpdate, T entity)
    {
        throw new NotImplementedException();
    }

    public virtual async Task DeleteAsync(T entity)
    {
        throw new NotImplementedException();
    }
}