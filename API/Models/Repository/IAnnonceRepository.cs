using API.DTO.Annonce;

namespace API.Models.Repository;

public interface IAnnonceRepository<TEntity, TIdentifier, TFilterEntity> : IDataRepository<TEntity, TIdentifier>
{
    
    Task<IEnumerable<TEntity>> GetByCategorieId(TIdentifier id);
    Task<IEnumerable<TEntity>> GetBySousCategorieId(TIdentifier id);
    Task<IEnumerable<TEntity>> GetActiveAnnonces();
    Task<IEnumerable<TEntity>> GetByUtilisateurId(TIdentifier id);
    
    Task<IEnumerable<TEntity>> GetByUtilisateurFavoris(TIdentifier id);
    Task<IEnumerable<TEntity>> SearchAsync(AnnonceSearchRequestDTO request);

    Task<IEnumerable<TEntity>> GetMostRecentAsync();
    Task<IEnumerable<TEntity>> GetPlusLikeAsync();
    Task<IEnumerable<TEntity>> FilterAsync(TFilterEntity filterDto);
}