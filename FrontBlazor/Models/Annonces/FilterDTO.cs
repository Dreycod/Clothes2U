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
   public string? Marque { get; set; }
   public string? Categorie { get; set; }
   public string? SousCategorie { get; set; }
   public string? Taille { get; set; }
   public double? Prix { get; set; }
   
   public SortField? SortBy { get; set; }
   public SortOrder SortOrder { get; set; } = SortOrder.Ascending;
}