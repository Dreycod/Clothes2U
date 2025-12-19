using System.Text.Json.Serialization;
using Shared.DTO.Utilisateur;

namespace Shared.DTO.LoginRegister;

public class LoginResponseDTO
{
    [JsonPropertyName("utilisateur")]
    public UtilisateurDTO utilisateur { get; set; }

    [JsonPropertyName("token")]
    public string token { get; set; }
}