using System.Text.Json.Serialization;

namespace Shared.DTO.Annonce;

public class AnnonceSuggestionDTO
{
    [JsonPropertyName("nomMarque")]
    public string NomMarque { get; set; }
    
    [JsonPropertyName("categorie")]
    public string Categorie { get; set; }
    
    [JsonPropertyName("sousCategorie")]
    public string SousCategorie { get; set; }
    
    [JsonPropertyName("prix")]
    public double Prix { get; set; }
    
    [JsonPropertyName("taille")]
    public string Taille { get; set; }
    
    [JsonPropertyName("dateAnnonce")]
    public DateTime DateAnnonce { get; set; }
    
    [JsonPropertyName("etatArticle")]
    public string EtatArticle { get; set; }
    [JsonPropertyName("couleurs")]
    public List<string> Couleurs { get; set; }
}