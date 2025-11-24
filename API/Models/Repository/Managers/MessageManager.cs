using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class MessageManager :  GenericCRUDManager<Message>
{
    public MessageManager(Clothes2UDbContext context) : base(context){}
    
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