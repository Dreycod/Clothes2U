using System.Text.Json.Serialization;

namespace FrontBlazor.Models.LoginRegister;

public class LoginResponse
{
    [JsonPropertyName("utilisateur")]
    public Utilisateur utilisateur { get; set; }

    [JsonPropertyName("token")]
    public string token { get; set; }
}