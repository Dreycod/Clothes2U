using FrontBlazor.Services.GenericIServices;
using Shared.DTO.Recense;

namespace FrontBlazor.Services.Interfaces
{
    public interface IRecenseService<TEntity> : IReadableService<TEntity> where TEntity : class
    {
        Task<List<TEntity>> GetTagsByAnnonce(int id);
    }
}
