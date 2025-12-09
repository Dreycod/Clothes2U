using System.Net;
using FrontBlazor.Models;
using System.Net.Http;
using FrontBlazor.Models.LoginRegister;

namespace FrontBlazor.Services.GenericIServices;

public interface IAuthService
{
    public Task<HttpStatusCode> LoginAsync(LoginRequest compte);
    public Task<AuthResult> SignUpAsync(LoginRequest compte);
    public Task LogoutAsync();
    public Task<Utilisateur?> GetCurrentUserAsync();
}