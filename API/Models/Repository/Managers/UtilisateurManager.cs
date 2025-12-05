using API.Models.EntityFramework;

namespace API.Models.Repository.Managers;

public class UtilisateurManager : GenericCRUDManager<Utilisateur>, IUtilisateurRepository
{
    public UtilisateurManager(Clothes2UDbContext context) : base(context){}

    public async Task UpdatePassword(Utilisateur utilisateur, string newPassword)
    {
        utilisateur.Password = newPassword;
        await _context.SaveChangesAsync();
        
    }
    
}