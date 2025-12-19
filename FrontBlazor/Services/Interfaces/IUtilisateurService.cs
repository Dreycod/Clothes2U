using Shared.DTO;
using FrontBlazor.Services.GenericIServices;
using Shared.DTO.Utilisateur;

namespace FrontBlazor.Services.Interfaces;

public interface IUtilisateurService : IReadableService<UtilisateurViewDTO>
{
    Task<UtilisateurViewDTO?> GetByLoginAsync(string login);
    Task<UtilisateurViewDTO> GetUserById(int id); // type of T.Name marche pas car UtilisaterView =/= Utilisateur
}