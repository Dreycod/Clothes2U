using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class ConversationManager : GenericCRUDManager<Conversation>, IConversationRepository<Conversation, int>
{
    public ConversationManager(Clothes2UDbContext context) : base(context){}

    private IQueryable<Conversation> BaseConversationQuery()
    {
        return _context.Conversations
            .Include(a => a.Vendeur)
            .ThenInclude(v => v.UtilisateurVendeur)
            .Include(a => a.Acheteur)
            .ThenInclude(a => a.UtilisateurAcheteur)
            .Include(a => a.LAnnonce)
            .Include(a => a.Messages)
            .ThenInclude(t => t.MessageTexte)
            .Include(a => a.Messages)
            .ThenInclude(u => u.Utilisateur);
            
    }


    public async Task<Conversation?> GetByIdAsync(int id)
    {
        return await BaseConversationQuery()
            .FirstOrDefaultAsync(a => a.ConversationId == id);
    }

    public async Task<IEnumerable<Conversation>> GetAllAsyncByUser(int id)
    {
        return await BaseConversationQuery()
            .Where(a => a.Vendeur.UtilisateurVendeurId == id || a.Acheteur.UtilisateurAcheteurId == id)
            .ToListAsync();
    }

    public async Task<int?> GetOtherUser(int currentUserId, Conversation conversation)
    {
        int? otherUserId = null;
        if (conversation.Acheteur.UtilisateurAcheteurId == currentUserId)
        {
            otherUserId = conversation.Vendeur?.UtilisateurVendeurId;
        }
            
        else if (conversation.Vendeur.UtilisateurVendeurId == currentUserId)
        {
            otherUserId = conversation.Acheteur?.UtilisateurAcheteurId;
        }
        return otherUserId;
    }
    
}