using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class CouleurManager : GenericCRUDManager<Couleur>
{
    public  CouleurManager(Clothes2UDbContext context) : base(context){}
}