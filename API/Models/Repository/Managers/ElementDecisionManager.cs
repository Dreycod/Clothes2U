using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class ElementDecisionManager : GenericCRUDManager<ElementDecision>, IDataRepository<ElementDecision, int>
{
    public ElementDecisionManager(Clothes2UDbContext context) : base(context){}
}