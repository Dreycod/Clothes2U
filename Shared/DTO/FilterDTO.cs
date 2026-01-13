namespace Shared.DTO;

public enum SortField
{
    DateAnnonce,
    Prix,
    Titre,
    NombreFavoris
}

public enum SortOrder
{
    Ascending,
    Descending
}

public class FilterDTO
{
    public string? MotCle { get; set; }
    
    // Listes au lieu de valeurs uniques
    public List<string>? Marques { get; set; }
    public string? Categories { get; set; }
    public string? SousCategories { get; set; }
    public string? Tailles { get; set; }
    public List<string>? Etats { get; set; }
    public List<string>? Genre { get; set; }
    public List<string>? Couleurs { get; set; }
    
    // Plage de prix
    public double? PrixMin { get; set; }
    public double? PrixMax { get; set; }
    
    public SortField? SortBy { get; set; }
    public SortOrder SortOrder { get; set; } = SortOrder.Ascending;
}