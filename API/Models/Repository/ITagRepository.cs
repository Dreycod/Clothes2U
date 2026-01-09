using Shared.DTO.Tag;

namespace API.Models.Repository
{
    public interface ITagRepository<TEntity, TIdentifier> : IDataRepository<TEntity, TIdentifier>
    {
        Task<TEntity?> GetByTagId(TIdentifier id);
        Task<IEnumerable<TEntity>> GetAllTagsAsync();
        Task<IEnumerable<TEntity>> SearchAsync(CreateTagDTO request);
    }
}