using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class SousCategorieManager : GenericCRUDManager<SousCategorie>
{
    public SousCategorieManager(Clothes2UDbContext context) : base(context){}
}