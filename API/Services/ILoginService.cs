using API.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace API.Services;

public interface ILoginService
{
    protected Task<Utilisateur> AuthentificateUtilisateur(string login, string password);
    protected Task<string> GenerateJwtToken(Utilisateur utilisateur);
}