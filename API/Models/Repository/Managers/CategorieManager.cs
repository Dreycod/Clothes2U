using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class CategorieManager : GenericCRUDManager<Categorie>
{
    public CategorieManager(Clothes2UDbContext context) : base(context){}
}