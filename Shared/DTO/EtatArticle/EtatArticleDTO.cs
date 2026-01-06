using System.ComponentModel.DataAnnotations.Schema;
using Shared.Interfaces;

namespace Shared.DTO.EtatArticle;

public class EtatArticleDTO : IEntity
{
    public int EtatArticleId { get; set; }
    public String NomEtat { get; set; } = null!;
    public int GetId()
    {
        return EtatArticleId;
    }
}
