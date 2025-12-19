using System.ComponentModel.DataAnnotations.Schema;

namespace Shared.DTO.EtatArticle;

public class EtatArticleDTO
{
    public int EtatArticleId { get; set; }
    public String NomEtat { get; set; } = null!;
    public int GetId()
    {
        return EtatArticleId;
    }
}
