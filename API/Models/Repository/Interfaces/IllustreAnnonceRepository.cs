namespace API.Models.Repository.Managers;

public interface IllustreAnnonceRepository<TEntity, TIdentifier> : IDataRepository<TEntity, TIdentifier>
{
    Task<IEnumerable<TEntity>?> GetByPhotoIds(IEnumerable<TIdentifier> photoId);
    Task<TEntity?> GetByPhotoId(TIdentifier photoId);

}