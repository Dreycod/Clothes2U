using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class StatutConversationManager : GenericCRUDManager<StatutConversation>
{
    public StatutConversationManager(Clothes2UDbContext context) : base(context){}
}