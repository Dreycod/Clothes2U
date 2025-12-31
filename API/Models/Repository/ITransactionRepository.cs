using Shared.DTO.Transaction;
using Shared.DTO.Historique;

namespace API.Models.Repository
{
    public interface ITransactionRepository<TEntity, TIdentifier> : IDataRepository<TEntity, TIdentifier>
    {
        Task<IEnumerable<TEntity>> GetByIdConversation(int conversationId);
        Task<IEnumerable<TEntity>> GetByIdTransaction (int transactionId);
        Task<IEnumerable<TransactionHistoriqueDTO>> GetHistoriqueAcheter(int utilisateurId);
        Task<IEnumerable<TransactionHistoriqueDTO>> GetHistoriqueVendu(int utilisateurId);
    }
}
