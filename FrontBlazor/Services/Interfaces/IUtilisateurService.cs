using FrontBlazor.Services.Interfaces.GenericIServices;
using Shared;
using Shared.DTO;
using Shared.DTO.Utilisateur;

namespace FrontBlazor.Services.Interfaces;

public interface IUtilisateurService : IReadableService<UtilisateurViewDTO>
{
    Task<UtilisateurViewDTO?> GetByLoginAsync(string login);
    Task<UtilisateurViewDTO> GetUserById(int id); 
    Task UpdateNotifMailPreferenceAsync(int userId, bool preference);
    Task<NewsDTO> GetActivity();
    Task<UtilisateurSettingsDTO> GetUserSettingsById(int id);
    Task<bool> PatchUpdateUser(UtilisateurSettingsDTO updatedUser);
    public Task<APIResponse<object>> SuppressionCompte(AccountDeletionDTO password);
}