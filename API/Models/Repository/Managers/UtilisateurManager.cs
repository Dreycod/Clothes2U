using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class UtilisateurManager : GenericCRUDManager<Utilisateur>
{
    public UtilisateurManager(Clothes2UDbContext context) : base(context){}
    
}