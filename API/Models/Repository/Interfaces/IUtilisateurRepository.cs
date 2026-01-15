using API.Models.EntityFramework;
using Shared.DTO;

namespace API.Models.Repository;

public interface IUtilisateurRepository: IDataRepository<Utilisateur, int>
{
    Task UpdatePassword(Utilisateur utilisateur, string newPassword);
    Task<Utilisateur?> GetUtilisateurByLogin(string login);
    Task<Utilisateur?> GetUtilisateurByEmail(string email);

    Task BanUser(int id);
    Task SuspendUser(int id);
    Task UpdatePassword(int utilisateurId, string hashedPassword);

    /// <summary>
    /// Récupère un utilisateur par email OU login
    /// Optimisation : une seule requête au lieu de GetAllAsync() puis filtrage
    /// </summary>
    Task<Utilisateur?> GetUtilisateurByEmailOrLogin(string loginOrEmail);


}