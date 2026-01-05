using Shared.DTO;

namespace API.Models.Repository;

public interface IClusterRepository
{
    Task SaveClustersAsync(ClusteringResponseDTO clusteringResponse);
}