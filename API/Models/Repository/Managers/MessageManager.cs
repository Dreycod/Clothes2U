using API.Models.EntityFramework;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Models.Repository.Managers;

public class MessageManager :  GenericCRUDManager<Message>
{
    public MessageManager(Clothes2UDbContext context) : base(context){}

    private IQueryable<Message> BaseMessageQuery()
    {
        return _context.Messages
            .Include(m => m.MessageTexte)
            .ThenInclude(mt => mt.Photos)
            .Include(m => m.MessageDemande)
            .Include(m => m.MessageValidation)
            .AsSplitQuery();
    }

    public override Task<Message?> GetByIdAsync(int id)
    {
        return BaseMessageQuery().Where(m => m.MessageId == id).FirstOrDefaultAsync();
    }
    
}

public class MessageTexteManager : GenericCRUDManager<MessageTexte>
{
    public MessageTexteManager(Clothes2UDbContext context) : base(context) {}
}

public class MessageDemandeManager : GenericCRUDManager<MessageDemande>, IMessageDemandeRepository
{
    public MessageDemandeManager(Clothes2UDbContext context) : base(context) {}
    
    public Task<MessageDemande?> GetByMessageIdAsync(int messageId)
    {
        return _context.MessageDemandes
            .FirstOrDefaultAsync(md => md.MessageId == messageId);
    }
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