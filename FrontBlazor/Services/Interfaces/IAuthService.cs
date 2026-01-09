using System.Collections.ObjectModel;
using System.Net;
using Shared.DTO;
using System.Net.Http;
using Shared.DTO.LoginRegister;
using Shared.DTO.Utilisateur;

namespace FrontBlazor.Services.Interfaces;

public interface IAuthService
{
    public Task<HttpStatusCode> LoginAsync(LoginRequestDTO compte);
    public Task<AuthResult> SignUpAsync(RegisterRequestDTO compte);
    public Task LogoutAsync();
    public Task<UtilisateurViewDTO?> GetCurrentUserAsync();
    public string GetGoogleLoginUrl(string returnUrl = "/");
    public Task<bool> ModificationMotDePasse(ChangePasswordDTO passwordDTO);
    Task<List<AdresseDTO>> GetUserAddressesAsync();
    Task<AdresseDTO> AddAddressAsync(CreateAdresseDTO address);
    Task<bool> UpdateAddressAsync(int addressId, UpdateAdresseDTO address);
    Task<bool> DeleteAddressAsync(int addressId);
    Task<bool> SetDefaultAddressAsync(int addressId);
}