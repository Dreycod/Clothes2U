using API.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class MessageManager :  GenericCRUDManager<Message>
{
    public MessageManager(Clothes2UDbContext context) : base(context){}

    private IQueryable<Message> BaseAnnonceQuery()
    {
        return _context.Messages
            .Include(m => m.MessageTexte)
            .Include(m => m.MessageDemande)
            .Include(m => m.MessageValidation)
            .AsSplitQuery();
    }

    public async Task<Conversation?> GetByIdAsync(int id)
    {
        var conversation = await _context.Conversations
            .Include(c => c.Vendeur).ThenInclude(v => v.UtilisateurVendeur)
            .Include(c => c.Acheteur).ThenInclude(a => a.UtilisateurAcheteur)
            .Include(c => c.LAnnonce)
            .Include(c => c.Messages)
            .ThenInclude(m => m.Utilisateur)
            .Include(c => c.Messages)
            .ThenInclude(m => m.MessageTexte)
            .ThenInclude(mt => mt.Photos)
            .Include(c => c.Messages)
            .ThenInclude(m => m.MessageDemande)  // ✅ Vérifiez que ceci est bien présent
            .ThenInclude(md => md.Offre)      // Si vous en avez besoin
            .Include(c => c.Messages)
            .ThenInclude(m => m.MessageValidation)
            .ThenInclude(mv => mv.PropositionValidee)  // ✅ Important pour MessageValidation
            .AsSplitQuery()  // ✅ Recommandé pour éviter les cartesian explosions
            .FirstOrDefaultAsync(c => c.ConversationId == id);
        if (conversation != null)
        {
            foreach (var msg in conversation.Messages)
            {
                Console.WriteLine($"Message {msg.MessageId}:");
                Console.WriteLine($"  - MessageTexte: {(msg.MessageTexte != null ? "✓" : "✗")}");
                Console.WriteLine($"  - MessageDemande: {(msg.MessageDemande != null ? "✓" : "✗")}");
                Console.WriteLine($"  - MessageValidation: {(msg.MessageValidation != null ? "✓" : "✗")}");
            }
        }
    
        return conversation;
    }
}

public class MessageTexteManager : GenericCRUDManager<MessageTexte>
{
    public MessageTexteManager(Clothes2UDbContext context) : base(context) {}
}

public class MessageDemandeManager : GenericCRUDManager<MessageDemande>
{
    public MessageDemandeManager(Clothes2UDbContext context) : base(context) {}
}

public class MessageValidationManager : GenericCRUDManager<MessageValidation>
{
    public MessageValidationManager(Clothes2UDbContext context) : base(context) {}
}

public class MessageContientImageManager : GenericCRUDManager<MessageContientImage>
{
    public MessageContientImageManager(Clothes2UDbContext context) : base(context)
    {
    }
}