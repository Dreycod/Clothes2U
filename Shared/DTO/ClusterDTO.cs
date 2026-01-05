using System.Text.Json.Serialization;

namespace Shared.DTO;

public class CentroideDTO
{
    [JsonPropertyName("clusterId")]
    public int ClusterId { get; set; }
    
    [JsonPropertyName("nbAnnonces")]
    public int NbAnnonces { get; set; }
    
    [JsonPropertyName("prixMoyen")]
    public double PrixMoyen { get; set; }
    
    [JsonPropertyName("sousCategorie")]
    public string SousCategorie { get; set; }
    
    [JsonPropertyName("nomMarque")]
    public string NomMarque { get; set; }
    
    [JsonPropertyName("taille")]
    public string Taille { get; set; }
    
    [JsonPropertyName("etatArticle")]
    public string EtatArticle { get; set; }
    
    [JsonPropertyName("couleurDominante")]
    public string CouleurDominante { get; set; }
}

public class ClusterResultDTO
{
    [JsonPropertyName("categorie")]
    public string Categorie { get; set; }
    
    [JsonPropertyName("centroides")]
    public List<CentroideDTO> Centroides { get; set; }
}

public class ClusteringResponseDTO
{
    [JsonPropertyName("success")]
    public bool Success { get; set; }
    
    [JsonPropertyName("userId")]
    public int UserId { get; set; }
    
    [JsonPropertyName("nbAnnoncesTotal")]
    public int NbAnnoncesTotal { get; set; }
    
    [JsonPropertyName("nbCategories")]
    public int NbCategories { get; set; }
    
    [JsonPropertyName("clustersParCategorie")]
    public List<ClusterResultDTO> ClustersParCategorie { get; set; }
}