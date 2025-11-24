using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class IllustreAnnonceManager : GenericCRUDManager<Illustre_Annonce> 
{
    public IllustreAnnonceManager(Clothes2UDbContext context) : base(context){}
}