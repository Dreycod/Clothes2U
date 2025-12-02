using API.Models.EntityFramework;
using API.Models.Repository;

namespace API.Services;

public class LoginService : ILoginService
{
    private readonly IDataRepository<Utilisateur, int> _utilisateurRepository;
    
    public LoginService(IDataRepository<Utilisateur, int> utilisateurRepository)
    {
        _utilisateurRepository = utilisateurRepository;
    }
    
    

    public Task<Utilisateur> AuthentificateUtilisateur(string login, string password)
    {
        throw new NotImplementedException();
    }

    public Task<string> GenerateJwtToken(Utilisateur utilisateur)
    {
        throw new NotImplementedException();
    }
}