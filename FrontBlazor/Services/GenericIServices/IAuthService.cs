using FrontBlazor.Models;
using System.Net.Http;

namespace FrontBlazor.Services.GenericIServices;

public interface IAuthService
{
    public Task<AuthResult> LoginAsync(string loginOrEmail, string password);
    public Task<AuthResult> SignUpAsync(string email, string login, string password, string  passwordConfirmation);
    public Task LogoutAsync();
    public Task<Utilisateur?> GetCurrentUserAsync();
}
