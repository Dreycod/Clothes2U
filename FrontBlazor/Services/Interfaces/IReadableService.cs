using FrontBlazor.Models;

namespace FrontBlazor.Services.GenericIServices;

public interface IReadableService<TEntity> where TEntity : class
{
    Task<TEntity> GetByIdAsync(int id);
}