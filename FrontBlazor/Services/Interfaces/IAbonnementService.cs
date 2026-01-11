using FrontBlazor.Services.Interfaces.GenericIServices;
using Shared.DTO.Utilisateur;

namespace FrontBlazor.Services.Interfaces;

public interface IAbonnementService
{
    Task AddAbonnement(int utilisateurId);
    Task DeleteAbonnement(int utilisateurId);
    Task<List<UtilisateurCardDTO>> GetAbonnements();
}
