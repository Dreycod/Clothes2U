using API.DTO;
using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public interface IPhotoRepository<TEntity, TIdentifier> : IDataRepository<TEntity, TIdentifier>
{
    Task<TEntity> GetByIdWithRelationsAsync(TIdentifier id);
    Task<TEntity> AddPhotoAsync(PhotoDTO photo);

}