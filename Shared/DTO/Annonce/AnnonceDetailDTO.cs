using System.Reflection;

namespace Shared.DTO.Annonce;

public class AnnonceDetailDTO : AnnonceDTO
{
    public List<string>? Tags { get; set; } = new();
    public string? SousCategorie { get; set; } 
    public string? Categorie { get; set; } 
    
    //id pour les posts : 
    
    public int EtatId { get; set; }
    public int MarqueId { get; set; }
    public int TailleId { get; set; }
    public int SousCategorieId { get; set; }
    public int CategorieId { get; set; }
    public int StatutAnnonceId { get; set; }

    public override bool Equals(object? obj)
    {
        if (obj == null || obj.GetType() != GetType())
            return false;

        var other = (AnnonceDetailDTO)obj;
        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var prop in properties)
        {
            var value1 = prop.GetValue(this);
            var value2 = prop.GetValue(other);

            if (value1 == null && value2 == null)
                continue;

            if (value1 == null || value2 == null)
                return false;

            if (!value1.Equals(value2))
                return false;
        }

        return true;
    }
}