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
            .AsSplitQuery();
    }

    public override async Task<Message?> GetByIdAsync(int id)
    {
        return await BaseAnnonceQuery()
            .FirstOrDefaultAsync(m => m.MessageId == id);
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