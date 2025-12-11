using API.DTO.Recense;

namespace API.Models.Repository
{
    public interface IRecenseRepository<TEntity, TIdentifier> : IDataRepository<TEntity, TIdentifier>
    {
        Task<IEnumerable<TEntity>> GetByRecenseId(TIdentifier id);
        Task<IEnumerable<TEntity>> GetAllAnnonceByTagId(TIdentifier id);
        Task<IEnumerable<TEntity>> GetAllTagByAnnonceId(TIdentifier id);
        Task<IEnumerable<TEntity>> SearchAsync(RecenseSearchRequestDTO request);

    }
}
