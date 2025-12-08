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
        return await _context.Conversations
            .Include(c => c.Vendeur).ThenInclude(v => v.UtilisateurVendeur)
            .Include(c => c.Acheteur).ThenInclude(a => a.UtilisateurAcheteur)
            .Include(c => c.LAnnonce)
            .Include(c => c.Messages)
            .ThenInclude(m => m.Utilisateur)
            .Include(c => c.Messages)
            .ThenInclude(m => m.MessageTexte)
            .ThenInclude(mt => mt.Photos)
            .Include(c => c.Messages)
            .ThenInclude(m => m.MessageDemande)
            .Include(c => c.Messages)
            .ThenInclude(m => m.MessageValidation)
            .FirstOrDefaultAsync(c => c.ConversationId == id);
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