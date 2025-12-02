using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FrontBlazor.Models;

public class EtatArticle : IEntity
{
    public int EtatArticleId { get; set; }
    
    public string? NomEtat { get; set; } = null;

    public int GetId()
    {
        return EtatArticleId;
    }
}