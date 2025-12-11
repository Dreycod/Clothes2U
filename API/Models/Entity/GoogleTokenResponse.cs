using System.Text.Json.Serialization;

namespace API.Models.Entity
{
    public class GoogleTokenResponse
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }
    }
}
