using System.ComponentModel.DataAnnotations.Schema;

namespace API.DTO.EtatArticle;

public class EtatArticleDTO
{
    public int EtatArticleId { get; set; }
    public String NomEtat { get; set; } = null!;
}
