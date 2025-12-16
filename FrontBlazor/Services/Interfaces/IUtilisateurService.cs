using FrontBlazor.Models;
using FrontBlazor.Services.GenericIServices;

namespace FrontBlazor.Services.Interfaces;

public interface IUtilisateurService : IReadableService<UtilisateurView>
{
    Task<UtilisateurView?> GetByLoginAsync(string login);
}