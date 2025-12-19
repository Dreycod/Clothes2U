using Shared.DTO;

namespace FrontBlazor.Services.GenericIServices;

public interface IReadableService<TEntity> where TEntity : class
{
    Task<TEntity> GetByIdAsync(int id);
}