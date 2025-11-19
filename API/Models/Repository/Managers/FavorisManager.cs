using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class FavorisManager : GenericCRUDManager<Favoris>
{
    public FavorisManager(Clothes2UDbContext context) :  base(context){}
}