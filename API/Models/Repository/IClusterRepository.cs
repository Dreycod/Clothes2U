using API.Models.EntityFramework;
using Shared.DTO;

namespace API.Models.Repository;

public interface IClusterRepository : IDataRepository<AnnoncePreferenceUtilisateur, int>
{
    Task SaveClustersAsync(ClusteringResponseDTO clusteringResponse);
    Task<IEnumerable<AnnoncePreferenceUtilisateur>> GetByUserId(int userId);
    Task DeleteByUserId(int userId);
}