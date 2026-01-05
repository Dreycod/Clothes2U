using System.Net;
using Shared.DTO;
using System.Net.Http;
using Shared.DTO.LoginRegister;
using Shared.DTO.Utilisateur;

namespace FrontBlazor.Services.GenericIServices;

public interface IAuthService
{
    public Task<HttpStatusCode> LoginAsync(LoginRequestDTO compte);
    public Task<AuthResult> SignUpAsync(RegisterRequestDTO compte);
    public Task LogoutAsync();
    public Task<UtilisateurViewDTO?> GetCurrentUserAsync();
    public string GetGoogleLoginUrl(string returnUrl = "/");
    public Task<bool> ModificationMotDePasse(ChangePasswordDTO passwordDTO);
}