using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class StatutAnnonceManager : GenericCRUDManager<StatutAnnonce>
{
    public StatutAnnonceManager(Clothes2UDbContext context) : base(context){}
}