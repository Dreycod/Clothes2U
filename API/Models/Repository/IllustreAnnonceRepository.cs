namespace API.Models.Repository.Managers;

public interface IllustreAnnonceRepository<TEntity, TIdentifier> : IDataRepository<TEntity, TIdentifier>
{
    Task<TEntity?> GetByPhotoId(TIdentifier photoId);
    
}