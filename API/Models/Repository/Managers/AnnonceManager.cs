using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class AnnonceManager : GenericCRUDManager<Annonce>
{
    public AnnonceManager(Clothes2UDbContext context) : base(context)
    {
    }
}