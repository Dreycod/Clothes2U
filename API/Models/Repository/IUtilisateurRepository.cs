using API.Models.EntityFramework;
using Shared.DTO;

namespace API.Models.Repository;

public interface IUtilisateurRepository: IDataRepository<Utilisateur, int>
{
    Task UpdatePassword(Utilisateur utilisateur, string newPassword);
    Task<Utilisateur> GetUtilisateurByLogin(string login);
    Task<int> GetSuspendUserCount();
    Task<Utilisateur?> GetUtilisateurByEmail(string email);
    
    //Task<ICollection<AdresseDTO>> GetUserAddresses(int utilisateurId);

}