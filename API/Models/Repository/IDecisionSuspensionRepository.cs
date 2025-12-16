using API.DTO.Decision_suspension;

namespace API.Models.Repository
{
    public interface IDecisionSuspensionRepository<TEntity, TIdentifier>
        : IDataRepository<TEntity, TIdentifier>
    {
        Task<IEnumerable<TEntity>> GetAllAsyncByUser(TIdentifier utilisateurId);
        Task<IEnumerable<TEntity>> SearchAsync(DecisionSuspensionSearchRequestDTO request);
    }
}
