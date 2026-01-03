using Shared.DTO.Historique;
using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers
{
    public class TransactionManager : GenericCRUDManager<Transaction>, ITransactionRepository<Transaction, int>
    {
        public TransactionManager(Clothes2UDbContext context) : base(context) { }

        private IQueryable<Transaction> BaseQuery()
        {
            return _context.Transactions
                .Include(t => t.Conversation)
                .Include(t => t.Conversation.Acheteur)
                .Include(t => t.Conversation.Vendeur)
                .Include(t => t.Conversation.LAnnonce)
                .AsSplitQuery();
        }

        public async Task<IEnumerable<Transaction>> GetByIdConversation(int conversationId)
        {
            return await BaseQuery()
                .Where(t => t.ConversationId == conversationId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaction>> GetByIdTransaction(int transactionId)
        {
            return await BaseQuery()
                .Where(t => t.TransactionId == transactionId)
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionHistoriqueDTO>> GetHistoriqueAcheter(int utilisateurId)
        {
            return await BaseQuery()
                .Where(t => t.Conversation.Acheteur.UtilisateurAcheteurId == utilisateurId)
                .Select(t => new TransactionHistoriqueDTO
                {
                    TransactionId = t.TransactionId,
                    Montant = t.TransactionMontant,
                    TypeTransaction = "Acheteur",
                    DateDebutNegociation = t.Conversation.CreationDate,
                    UtilisateurId = utilisateurId
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<TransactionHistoriqueDTO>> GetHistoriqueVendu(int utilisateurId)
        {
            return await BaseQuery()
                .Where(t => t.Conversation.Vendeur.UtilisateurVendeurId == utilisateurId)
                .Select(t => new TransactionHistoriqueDTO
                {
                    TransactionId = t.TransactionId,
                    Montant = t.TransactionMontant,
                    TypeTransaction = "Vendeur",
                    DateDebutNegociation = t.Conversation.CreationDate,
                    UtilisateurId = utilisateurId
                })
                .ToListAsync();
        }

    }
}
