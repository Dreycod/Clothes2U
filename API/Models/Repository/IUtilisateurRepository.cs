using API.Models.EntityFramework;

namespace API.Models.Repository;

public interface IUtilisateurRepository: IDataRepository<Utilisateur, int>
{
    Task UpdatePassword(Utilisateur utilisateur, string newPassword);
    Task<Utilisateur> GetUtilisateurByLogin(string login);
    Task<int> GetSuspendUserCount();
}