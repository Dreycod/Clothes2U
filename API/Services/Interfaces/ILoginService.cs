using API.Models.EntityFramework;
using Microsoft.AspNetCore.Mvc;

namespace API.Services;

public interface ILoginService
{
    (AuthResult result, Utilisateur? user) AuthenticateUtilisateur(string loginOrEmail, string password, List<Utilisateur> users);
    string GenerateJwtToken(Utilisateur utilisateur);
}