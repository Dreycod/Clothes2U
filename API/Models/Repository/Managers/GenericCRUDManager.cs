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
        T entity = _context.Set<T>().AsEnumerable().FirstOrDefault(e => e.GetId() == id);
        return entity;
    }

    public virtual async Task AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T entityToUpdate, T entity)
    {
        _context.Set<T>().Attach(entityToUpdate);
        _context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(T entity)
    {
        _context.Set<T>().Remove(entity);
        await _context.SaveChangesAsync();
    }
}