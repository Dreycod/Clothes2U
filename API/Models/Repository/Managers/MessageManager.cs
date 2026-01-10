using API.Models.EntityFramework;
using API.Models.Repository.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class MessageManager :  GenericCRUDManager<Message>, IMessageRepository
{
    public MessageManager(Clothes2UDbContext context) : base(context){}

    private IQueryable<Message> BaseMessageQuery()
    {
        return _context.Messages
            .Include(m => m.MessageTexte)
            .ThenInclude(mt => mt.Photos)
            .Include(m => m.MessageDemande)
            .Include(m => m.MessageEstPayee)
            .AsSplitQuery();
    }

    public override Task<Message?> GetByIdAsync(int id)
    {
        return BaseMessageQuery().Where(m => m.MessageId == id).FirstOrDefaultAsync();
    }

    public async Task<int> GetMessageCountByUserId(int id)
    {
        return await _context.Messages
            .Where(m => m.MessageStatut == true && 
                        m.MessageLu == false && 
                        m.UtilisateurId != id && 
                        (m.Conversation.Acheteur.UtilisateurAcheteurId == id || 
                        m.Conversation.Vendeur.UtilisateurVendeurId == id))
            .CountAsync();
    }
}

public class MessageTexteManager : GenericCRUDManager<MessageTexte>
{
    public MessageTexteManager(Clothes2UDbContext context) : base(context) {}
}

public class MessageDemandeManager : GenericCRUDManager<MessageDemande>, IMessageDemandeRepository
{
    public MessageDemandeManager(Clothes2UDbContext context) : base(context) {}
    
    public async Task<MessageDemande?> GetByMessageIdAsync(int messageId)
    {
        return await _context.MessageDemandes
            .Include(md => md.Message)              // Charge le Message
            .ThenInclude(m => m.Conversation)   // Charge la Conversation du Message
            .FirstOrDefaultAsync(md => md.MessageId == messageId);
    }
}

public class MessageEstPayeeManager : GenericCRUDManager<MessageEstPayee>
{
    public MessageEstPayeeManager(Clothes2UDbContext context) : base(context) {}
}

public class MessageContientImageManager : GenericCRUDManager<MessageContientImage>
{
    public MessageContientImageManager(Clothes2UDbContext context) : base(context)
    {
    }
}

public class MessageEnvoieColisManager : GenericCRUDManager<MessageEnvoieColis>
{
    public MessageEnvoieColisManager(Clothes2UDbContext context) : base(context) {}
}

public class MessageEstRecuManager : GenericCRUDManager<MessageEstRecu>
{
    public MessageEstRecuManager(Clothes2UDbContext context) : base(context) {}
}