using API.DTO.Visualisation;

namespace API.Models.Repository
{
    public interface IVisualisationRepository<TEntity, TIdentifier> : IDataRepository<TEntity, TIdentifier>
    {
        Task<bool> HasRecentView(int utilisateurId, int annonceId, TimeSpan delay);
        Task<IEnumerable<TEntity>> GetByUtilisateurId(int utilisateurId);
        Task<IEnumerable<TEntity>> GetByAnnonceId(int annonceId);
        Task<int> CountViewsByAnnonce(int annonceId);
        Task<IEnumerable<TopAnnonceStatsDTO>> GetTopAnnonces(int limit);

    }
}
