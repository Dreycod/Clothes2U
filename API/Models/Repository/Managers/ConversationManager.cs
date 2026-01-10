using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class ConversationManager : GenericCRUDManager<Conversation>, IConversationRepository<Conversation, int>
{
    public ConversationManager(Clothes2UDbContext context) : base(context){}

    private IQueryable<Conversation> BaseConversationQuery()
    {
        return _context.Conversations
            .Include(c => c.Vendeur)
            .ThenInclude(v => v.UtilisateurVendeur)
            .Include(c => c.Acheteur)
            .ThenInclude(a => a.UtilisateurAcheteur)
            .Include(c => c.LAnnonce)
            .ThenInclude(a => a.Photos)
            .ThenInclude(p => p.Photo)
            .Include(c => c.Messages)
            .ThenInclude(m => m.Utilisateur)
            .Include(c => c.Messages)
            .ThenInclude(m => m.MessageTexte)
            .ThenInclude(mt => mt.Photos)
            .Include(c => c.Messages)
            .ThenInclude(m => m.MessageDemande)
            .ThenInclude(md => md.Offre) 
            .Include(c => c.Messages)
            .ThenInclude(m => m.MessageEstPayee)
            .Include(c => c.Messages)
            .ThenInclude(m => m.MessageEnvoieColis)
            .Include(c => c.Messages)
            .AsSplitQuery();
    }


    public async Task<Conversation?> GetByIdAsync(int id)
    {
        return await BaseConversationQuery()
            .FirstOrDefaultAsync(a => a.ConversationId == id);
    }

    public async Task<IEnumerable<Conversation>> GetAllAsyncByUser(int id)
    {
        return await BaseConversationQuery()
            .Where(c => 
                (c.Vendeur.UtilisateurVendeurId == id || c.Acheteur.UtilisateurAcheteurId == id) &&
                (c.Vendeur.UtilisateurVendeurId == id 
                    ? !c.Acheteur.UtilisateurAcheteur.UtilisateursBloques.Any(b => b.UtilisateurBloqueId == id) &&
                      !c.Acheteur.UtilisateurAcheteur.BloqueParUtilisateurs.Any(b => b.UtilisateurBloqueurId == id)
                    : !c.Vendeur.UtilisateurVendeur.UtilisateursBloques.Any(b => b.UtilisateurBloqueId == id) &&
                      !c.Vendeur.UtilisateurVendeur.BloqueParUtilisateurs.Any(b => b.UtilisateurBloqueurId == id)) &&
                (c.Vendeur.UtilisateurVendeurId == id 
                    ? c.Acheteur.UtilisateurAcheteur.Statut.StatutLibelle == "Actif"
                    : c.Vendeur.UtilisateurVendeur.Statut.StatutLibelle == "Actif")
            )
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
    
    public async Task<Conversation?> GetByUserAndAnnonceAsync(int userId, int annonceId)
    {
        return await BaseConversationQuery()
            .Where(c =>
                c.AnnonceId == annonceId &&
                (c.Acheteur.UtilisateurAcheteurId == userId ||
                 c.Vendeur.UtilisateurVendeurId == userId))
            .FirstOrDefaultAsync();
    }
    
    public async Task SuspendElement(int id)
    {
        Message message = _context.Messages.Find(id);
        message.MessageStatut = false;
        _context.Messages.Update(message);
        await _context.SaveChangesAsync();
    }
    
}