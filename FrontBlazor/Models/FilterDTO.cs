namespace FrontBlazor.Models;
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

    public List<string>? Marques { get; set; }
    public List<string>? Categories { get; set; }
    public List<string>? SousCategories { get; set; }
    public List<string>? Tailles { get; set; }
    public List<string>? Etats { get; set; }

    public double? PrixMin { get; set; }
    public double? PrixMax { get; set; }

    public SortField? SortBy { get; set; }
    public SortOrder SortOrder { get; set; } = SortOrder.Ascending;
}

